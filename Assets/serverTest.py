import socket
import cv2
import numpy as np
import os
# Server configuration
HOST = '0.0.0.0'  # Listen on all available network interfaces
PORT = 8080



def receive_screenshot_and_display():
    # Create a socket server
    server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    server_socket.bind((HOST, PORT))
    server_socket.listen(1)  # Listen for one incoming connection

    print(f"Server is listening on {HOST}:{PORT}...")
    count = 0 
    
    try:
        while True:
            # Accept a client connection
            client_socket, client_address = server_socket.accept()
            print(f"Accepted connection from {client_address}")

            # Receive the screenshot data
            screenshot_data = b''
            while True:
                chunk = client_socket.recv(1024)
                if not chunk:
                    break
                screenshot_data += chunk

            # Save the received screenshot to a file (e.g., 'received_screenshot.png')
            with open('received_screenshot.png', 'wb') as file:
                file.write(screenshot_data)

            print("Screenshot received and saved.")

            # Load and display the received image using OpenCV
            received_image = cv2.imread('received_screenshot.png') 
            

        
            if received_image is not None: 
                # Get the dimensions of the received image
                height, width, _ = received_image.shape

                # Turn half of the image red
                received_image[:, width // 2:, :] = [0, 0, 255] 
                print(type(received_image))  
                # Get the directory where your Python script is located
                script_directory = os.path.dirname(os.path.abspath(__file__))

                # Specify the output directory within the script's directory
                output_directory = os.path.join(script_directory, "output_images")
                if not os.path.exists(output_directory):
                    os.makedirs(output_directory)

                # Specify the output filename
                output_filename = os.path.join(output_directory, "output_image" + str(count) + ".png") 
                count += 1

                # Save the NumPy array as an image in the output directory
                cv2.imwrite(output_filename, received_image)
                cv2.imshow('Received Screenshot', received_image)
                cv2.waitKey(0)
                cv2.destroyAllWindows()
            else:
                print("Failed to load the received image.") 
                continue

            # Optionally, you can send a response back to the client if needed.
            # response = "Screenshot received successfully"
            # client_socket.send(response.encode())

            # Close the client socket
            client_socket.close()

    except KeyboardInterrupt:
        pass
    finally:
        server_socket.close()

if __name__ == "__main__":
    receive_screenshot_and_display()
