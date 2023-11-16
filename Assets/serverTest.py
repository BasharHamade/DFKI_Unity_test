import socket

server = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
server.bind(('0.0.0.0', 12345))
server.listen(1)

print("Server is listening...")
i = 0
while True:
    conn, addr = server.accept()
    print(f"Connection from {addr}")
    
    data = conn.recv(1024)
    if data:
        print("Received: ")
        conn.send("Message received".encode('utf-8')) 
        
    
conn.close()  # Close the connection after handling one data packet
