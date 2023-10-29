# DFKI_test_Unity
 Test Scene to create a connection between a Unity C# client and a python socket server to exchange images

-On the GameObject Object the clientTest is attached,which captures a screenshot of the first person view of the player when the "S" button is pressed and sends it using a tcp connection to the python socket server,it also saves the image locally at the current project directory 

-the serverTest python script is the script responsible for the socket server.It listens for a client connection and waits to recieve the image from the client.then it reads the image,makes half of it red(just for testing purposes) and displays that modified image using opencv,and saves the image in the processed_images folder in the project directory,so that the cliemt can have access to it 
Note:I attempted multiple ways to send back the image from the socket to be recieved by the client on the socket connection but faced a lot of issues which mainly involved the unity program freezing completely or not recieving the image back.

-the player is just a capsule with a simple first person controller script
