# [Windows] Install MinGW (https://sourceforge.net/projects/mingw), add C:\MinGW\bin to Environment Variables PATH. Run command mingw32-make
# [MacOS] https://discussions.apple.com/thread/5250041
# [Ubuntu] "sudo apt install build-essential"

EXECUTABLE = main

CC = "gcc"

all: server client

# clean for Windows and Linux
ifdef OS
   RM = del /Q
   LD = -lws2_32
   FixPath = $(subst /,\,$1)
else
   ifeq ($(shell uname), Linux)
      RM = rm -f
      LD =
      FixPath = $1
   endif
endif

server: server.o
	$(CC) server.c -g -o server $(LD)

client: client.o
	$(CC) client.c -g -o client $(LD)

.PHONY: clean
clean:
	$(RM) *.o client server