CC = "gcc"

all: server client

server: src/server.o
	$(CC) src/server.c -g -o server

client: src/client.o
	$(CC) src/client.c -g -o client