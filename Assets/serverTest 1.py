import socket
import cv2
import numpy as np
import base64

server = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
server.bind(('0.0.0.0', 12345))
server.listen(1)

print("Server is listening...")
conn, addr = server.accept()
print(f"Connection from {addr}")

while True:
    data = conn.recv(99999).decode()
    if not data:
        break
    print(f"Received: {data[0:20]}") 
    print("Length : " + str(len(data))) 
    # Decode the base64 string
    image_data = base64.b64decode(data)

    # Convert the binary data to a NumPy array
    image_array = np.frombuffer(image_data, np.uint8)

    # Decode the image using OpenCV
    image = cv2.imdecode(image_array, cv2.IMREAD_COLOR)

    # Display the image
    cv2.imshow("Base64 Image", image)
    cv2.waitKey(0)
    cv2.destroyAllWindows()

    
 
    
    conn.send("Message received".encode())

conn.close()
