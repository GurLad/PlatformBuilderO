import socket
import json
import math
import os
import threading
from SocketFunctions import *

print("Version 1")

hostname = socket.gethostname()    
IPAddr = socket.gethostbyname(hostname)

SERVER_IP = IPAddr
PORT = 4242

IPParts = SERVER_IP.split('.')
tempString = ''
for i in IPParts:
	tempString += str("{0:0{1}X}".format(int(i),2))
print("Server ID: " + tempString)

onlineLevelsState = {}
onlineLevelsTileChanges = {}
onlineLevelsIDUpdates = {}

server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
server_socket.bind((SERVER_IP, PORT))
server_socket.listen(100)

def FileExists(fpath):  
    return os.path.isfile(fpath) and os.path.getsize(fpath) > 0

def SocketCommunication(client_socket):
	while True:
		request = RecieveOne(client_socket)#client_socket.recv(1024)
		print("Recieved request: " + request.decode('utf-8'))
		if request == b"SAVE_LEVEL":
			fileName = RecieveOne(client_socket)#client_socket.recv(1024)
			fileName = fileName.decode('utf-8') + ".pbl"
			print("Filename: " + fileName);
			#if fileName[len(fileName) - len("FILE_END"):] != "FILE_END":
			#	client_socket.send(b"Error")
			#	print("Error!")
			#	client_socket.close()
			#	continue
			#fileName = fileName[:len(fileName) - len("FILE_END")]
			print("Recieved: " + str(fileName))
			fileContent = RecieveLargeData(client_socket)
			#fileContent = client_socket.recv(1024)
			print("Recieved: " + fileContent)
			print("Writing...")
			if FileExists("data\\" + fileName):
				file = open("data\\" + fileName, 'r')
				temp = file.read().split('~')
				fileUsername = temp[0]
				temp = fileContent.split('~')
				newUsername = temp[0]
				#fileUsername != newUsername or 
				if (fileUsername != newUsername):
					SendOne(client_socket, b"No access! Changes not saved")
					print("Nope! Compare:\r\n" + fileUsername + ",\r\n" + newUsername + ".")
					file.close()
					client_socket.close();
					continue
			file = open("data\\" + fileName, 'w')
			file.write(fileContent)
			file.close()
			print("Sending data...")
			SendOne(client_socket, b"Data recieved successfully!")#client_socket.send(b"Data recieved successfully!")
			print("Finished!")
		elif request == b"SEEK_LEVEL":
			fileName = RecieveOne(client_socket)#client_socket.recv(1024)
			fileName = fileName.decode('utf-8') + ".pbl"
			#if fileName[len(fileName) - len("FILE_END"):] != "FILE_END":
			#	client_socket.send(b"Error")
			#	print("Error!")
			#	client_socket.close()
			#	continue
			#fileName = fileName[:len(fileName) - len("FILE_END")]
			print("Recieved: " + str(fileName))
			if not FileExists("data\\" + fileName):
				SendOne(client_socket, b"Nonexistant level")
			else:
				SendOne(client_socket, b"Sending...")
				file = open("data\\" + fileName, 'r')
				fileData = file.read()
				file.close()
				SendLargeData(client_socket, fileData)
		elif request == b"SEEK_PASSWORD":
			username = RecieveOne(client_socket).decode('utf-8')
			if not FileExists("data\\" + username + ".user"):
				open("data\\" + username + ".user", 'w').close()
			file = open("data\\" + username + ".user", 'r')
			password = file.read()
			SendOne(client_socket, password.encode('utf-8'))
			file.close()
		elif request == b"SAVE_PASSWORD":
			username = RecieveOne(client_socket).decode('utf-8')
			password = RecieveOne(client_socket).decode('utf-8')
			file = open("data\\" + username + ".user", 'w')
			file.write(password)
			file.close()
		elif request == b"SEEK_LEVELS_BY":
			username = RecieveOne(client_socket).decode('utf-8')
			result = ""
			for file in os.listdir("./data/"):
				if file.endswith(".pbl"):
					fileName = os.path.join("", file).split('.')[0]
					file = open("data\\" + fileName + ".pbl", 'r')
					temp = file.read().split('~')
					fileUsername = temp[0]
					if (fileUsername == username):
						result += fileName + ";"
			SendOne(client_socket, result.encode('utf-8'))
		elif request == b"SEEK_ONLINE_LEVELS":
			result = ""
			for i in onlineLevelsState.keys():
				result += i + ";"
			SendOne(client_socket, result.encode('utf-8'))
		elif request == b"TURN_ONLINE":
			levelName = RecieveOne(client_socket).decode('utf-8')
			onlineLevelsState[levelName] = []
			onlineLevelsState[levelName].append(RecieveOne(client_socket).decode('utf-8'))
			onlineLevelsTileChanges[levelName] = []
			onlineLevelsTileChanges[levelName].append([])
			onlineLevelsIDUpdates[levelName] = []
		elif request == b"JOIN_LEVEL":
			levelName = RecieveOne(client_socket).decode('utf-8')
			SendOne(client_socket, str(len(onlineLevelsState[levelName])).encode('utf-8'))
			onlineLevelsState[levelName].append(RecieveOne(client_socket).decode('utf-8'))
			onlineLevelsTileChanges[levelName].append([])
		elif request == b"MOVE_PLAYER":
			levelName = RecieveOne(client_socket).decode('utf-8')
			playerID = int(RecieveOne(client_socket).decode('utf-8'))
			onlineLevelsState[levelName][playerID] = RecieveOne(client_socket).decode('utf-8')
		elif request == b"SHOW_PLAYERS":
			levelName = RecieveOne(client_socket).decode('utf-8')
			playerID = int(RecieveOne(client_socket).decode('utf-8'))
			result = ""
			for i in range(len(onlineLevelsState[levelName])):
				if not i == playerID:
					result += str(i) + ":" + onlineLevelsState[levelName][i] + "|"
			SendOne(client_socket, result.encode('utf-8'))
		elif request == b"SEND_TILES":
			levelName = RecieveOne(client_socket).decode('utf-8')
			playerID = int(RecieveOne(client_socket).decode('utf-8'))
			tiles = RecieveOne(client_socket).decode('utf-8').split(';')
			tiles = tiles[:len(tiles) - 1]
			for i in range(len(onlineLevelsState[levelName])):
				if not i == playerID:
					onlineLevelsTileChanges[levelName][i].extend(tiles)
		elif request == b"SEEK_TILES":
			levelName = RecieveOne(client_socket).decode('utf-8')
			playerID = int(RecieveOne(client_socket).decode('utf-8'))
			SendLargeData(client_socket, (';'.join(onlineLevelsTileChanges[levelName][playerID])))
			onlineLevelsTileChanges[levelName][playerID] = []
		elif request == b"SEEK_ID":
			levelName = RecieveOne(client_socket).decode('utf-8')
			playerID = int(RecieveOne(client_socket).decode('utf-8'))
			print("List: " + str(onlineLevelsIDUpdates[levelName]))
			temp = playerID
			while temp in onlineLevelsIDUpdates[levelName]:
				onlineLevelsIDUpdates[levelName].remove(playerID)
				playerID -= 1
				print("\n\n\n\nNew ID: " + str(playerID) + "\nList: " + str(onlineLevelsIDUpdates[levelName]) + "\n\n\n\n")
			SendOne(client_socket, str(playerID).encode('utf-8'))
		elif request == b"EXIT_LEVEL":
			levelName = RecieveOne(client_socket).decode('utf-8')
			playerID = int(RecieveOne(client_socket).decode('utf-8'))
			onlineLevelsState[levelName].pop(playerID)
			onlineLevelsTileChanges[levelName].pop(playerID)
			onlineLevelsIDUpdates[levelName].extend(range(playerID + 1, len(onlineLevelsState[levelName]) + 1))
		elif request == b"QUIT":
			print("Goodbye!")
			break
		else:
			SendOne(client_socket, "Hello".encode('utf-8'))

	client_socket.close()

while True:
	print("Waiting...")

	client_socket, address = server_socket.accept()
	print("Connected!")
	thread = threading.Thread(target=SocketCommunication, args=[client_socket], name="newThread")
	thread.start()