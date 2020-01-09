import socket
import json
import math
import os
from SocketFunctions import *

hostname = socket.gethostname()    
IPAddr = socket.gethostbyname(hostname)

SERVER_IP = IPAddr
PORT = 4242

IPParts = SERVER_IP.split('.')
tempString = ''
for i in IPParts:
	tempString += str("{0:0{1}X}".format(int(i),2))
print("Server ID: " + tempString)

server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
server_socket.bind((SERVER_IP, PORT))
server_socket.listen(100)

def FileExists(fpath):  
    return os.path.isfile(fpath) and os.path.getsize(fpath) > 0

while True:
	print("Waiting...")

	client_socket, address = server_socket.accept()
	print("Connected!")
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
	else:
		SendOne(client_socket, "Hello".encode('utf-8'))

	client_socket.close()