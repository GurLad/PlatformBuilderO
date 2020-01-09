import math
import socket

def SendLargeData(clientSocket, data):
	fileSize = len(data) / 1024.0
	fileSize = math.ceil(fileSize)
	SendOne(clientSocket, str(int(fileSize)).encode('utf-8'))
	toSend = data.encode('utf-8')
	for i in range(int(fileSize)):
		SendOne(clientSocket, toSend[:1024])
		print("Sent part " + str(i) + ": " + toSend[:1024].decode('utf-8'))
		toSend = toSend[1024:]
	print("Sent data")

def RecieveLargeData(clientSocket):
	fileSize = int(RecieveOne(clientSocket).decode('utf-8'))
	fileContent = ''
	for i in range(fileSize):
		fileContent += RecieveOne(clientSocket).decode('utf-8')
	return fileContent

def SendOne(clientSocket, dataToSend):
	size = len(dataToSend)
	clientSocket.send(str(int(size)).encode('utf-8') + '\a'.encode('utf-8'))
	clientSocket.send(dataToSend)

def RecieveOne(clientSocket):
	size = ''
	char = clientSocket.recv(1)
	while not char == '\a'.encode('utf-8'):
		size += char.decode('utf-8')
		char = clientSocket.recv(1)
	print(size)
	size = int(size)
	final = ''.encode('utf-8')
	while (len(final) < size):
		final += clientSocket.recv(size - len(final))
	return final
