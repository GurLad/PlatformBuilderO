import socket
import json
import math
import os
import threading
import sqlite3
from SocketFunctions import *

print("Platform Builder O Server - Version 1.2, 11/05/2020, by Gur Ladizhinsky")

hostname = socket.gethostname()    
IPAddr = socket.gethostbyname(hostname)

SERVER_IP = IPAddr
PORT = 4242
DATABASE_NAME = "Database.db"

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

def FindUserPassword(username):
	header = sqlite3.connect(DATABASE_NAME)
	curs = header.cursor()
	curs.execute("SELECT * FROM Users WHERE Name = (?)", (username,))
	data = curs.fetchall()
	if (len(data) == 0):
		return ""
	return data[0][1]

def SaveUserPassword(username, password):
	header = sqlite3.connect(DATABASE_NAME)
	curs = header.cursor()
	curs.execute("INSERT INTO Users VALUES(?, ?, ?)", (username, password, ''))
	header.commit()
	
def SaveUserLevel(username, levelName):
	header = sqlite3.connect(DATABASE_NAME)
	curs = header.cursor()
	curs.execute("SELECT * FROM Users WHERE Name = (?)", (username,))
	data = curs.fetchall()
	if (data[0][2] == ""):
		curs.execute("UPDATE Users SET Levels = (?) WHERE Name = (?)", (levelName, username))
		header.commit()
	else:
		curs.execute("UPDATE Users SET Levels = (?) WHERE Name = (?)", (data[0][2] + ";" + levelName, username))
		header.commit()
	
def SeekUserLevels(username):
	header = sqlite3.connect(DATABASE_NAME)
	curs = header.cursor()
	curs.execute("SELECT * FROM Users WHERE Name = (?)", (username,))
	data = curs.fetchall()
	return data[0][2]

def FileExists(fpath):  
    return os.path.isfile(fpath) and os.path.getsize(fpath) > 0

def SocketCommunication(client_socket):
	while True:
		request = RecieveOne(client_socket)#client_socket.recv(1024)
		print("Recieved request: " + request.decode('utf-8'))
		if request == b"SAVE_LEVEL":
			fileName = RecieveOne(client_socket)#client_socket.recv(1024)
			fileName = fileName.decode('utf-8') + ".pbl"
			print("Filename: " + fileName)
			#if fileName[len(fileName) - len("FILE_END"):] != "FILE_END":
			#	client_socket.send(b"Error")
			#	print("Error!")
			#	client_socket.close()
			#	continue
			#fileName = fileName[:len(fileName) - len("FILE_END")]
			print("Recieved: " + str(fileName))
			fileContent = RecieveLargeData(client_socket)
			temp = fileContent.split('~')
			newUsername = temp[0]
			#fileContent = client_socket.recv(1024)
			print("Recieved: " + fileContent)
			print("Writing...")
			if FileExists("data\\" + fileName):
				file = open("data\\" + fileName, 'r')
				temp = file.read().split('~')
				fileUsername = temp[0]
				#fileUsername != newUsername or 
				if (fileUsername != newUsername):
					SendOne(client_socket, b"No access! Changes not saved")
					print("Nope! Compare:\r\n" + fileUsername + ",\r\n" + newUsername + ".")
					file.close()
					continue
			else:
				SaveUserLevel(newUsername, fileName[:-4])
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
			password = FindUserPassword(username)
			SendOne(client_socket, password.encode('utf-8'))
		elif request == b"SAVE_PASSWORD":
			username = RecieveOne(client_socket).decode('utf-8')
			password = RecieveOne(client_socket).decode('utf-8')
			SaveUserPassword(username, password)
		elif request == b"SEEK_LEVELS_BY":
			username = RecieveOne(client_socket).decode('utf-8')
			result = SeekUserLevels(username)
			print(result)
			SendOne(client_socket, result.encode('utf-8'))
		elif request == b"SEEK_ONLINE_LEVELS":
			result = ""
			for i in onlineLevelsState.keys():
				result += i + ";"
			SendOne(client_socket, result.encode('utf-8'))
		elif request == b"TURN_ONLINE":
			levelName = RecieveOne(client_socket).decode('utf-8')
			if levelName in onlineLevelsState:
				SendOne(client_socket, b"This level is already online!")
				continue
			else:
				SendOne(client_socket, b"TURN")
			onlineLevelsState[levelName] = []
			onlineLevelsState[levelName].append(RecieveOne(client_socket).decode('utf-8'))
			onlineLevelsTileChanges[levelName] = []
			onlineLevelsTileChanges[levelName].append([])
			onlineLevelsTileChanges[levelName].append([])
			onlineLevelsIDUpdates[levelName] = []
		elif request == b"JOIN_LEVEL":
			levelName = RecieveOne(client_socket).decode('utf-8')
			if not levelName in onlineLevelsState:
				onlineLevelsState[levelName] = []
				onlineLevelsTileChanges[levelName] = []
				onlineLevelsIDUpdates[levelName] = []				
			SendOne(client_socket, str(len(onlineLevelsState[levelName])).encode('utf-8'))
			onlineLevelsState[levelName].append(RecieveOne(client_socket).decode('utf-8'))
			onlineLevelsTileChanges[levelName].append(onlineLevelsTileChanges[levelName][len(onlineLevelsTileChanges[levelName]) - 1])
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
			for i in range(len(onlineLevelsState[levelName]) + 1):
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
			if len(onlineLevelsState[levelName]) <= 0:
				onlineLevelsState.pop(levelName)
				onlineLevelsTileChanges.pop(levelName)
				onlineLevelsIDUpdates.pop(levelName)
				print(onlineLevelsState)
			else:
				print("Len: " + str(len(onlineLevelsState[levelName])))
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