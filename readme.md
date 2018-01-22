# A simple intro to Docker
<br><br><br>
## Step 1 - Install Docker:
This has been completed in advance.  Access your 'Docker' VM using Guacamole.
<br><br>

## Step 2 - Run a container using on the 'Hello World' image:
```
sudo docker run hello-world
```
<br>
Note the "Unable to find image 'hello-world:latest' locally" + "Pulling".
<br><br>

## Step 3 - Pull down an image fro Docker Hub before running a container:
```
sudo docker pull ubuntu:latest
```
<br><br>
## Step 4 - Run a container based on the image pulled down in Step 3 and attach:
```
sudo docker run -it ubuntu:latest /bin/bash
```
<br><br>
## Step 5 - Exit the container and clear up:
```
exit
```
or
<br>
'Ctrl + C'
<br>
```
sudo docker ps -a
```
```
sudo docker rm <idofcontainercopiedfromlastoutput>
```
<br><br>
## Step 6 - Run another container based on the image pulled down in Step 3, attach to it, install Nginx, test the service, and exit:

On your 'Docker VM':
```
ifconfig -a
```
Note down the IP address on 'Eth0'.

```
sudo docker run -it -p 80:80 --net=host ubuntu /bin/bash
```

On the container:
```
apt-get update
```
```
apt-get install nano -y
```
```
apt-get install nginx -y
```
```
nano /var/www/html/index.nginx-debian.html
```
Edit the html file in any way you see fit
<br>
'Ctrl+o', 'Ctrl+x' to save and exit nano
<br>
```
/etc/init.d/nginx start
```
<br>
On 'Mgmt VM' (Windows):
Browse to http://<dockervmeth0ipadress>

You should see an Nginx welcome page with your edits.
<br><br>
## Step 7 - Build a new image based on the addition of an additional layer atop the original image which was added in Step 6:

Back on the 'Docker VM':
'Ctrl-D' to exit the container

```
sudo docker ps -a
```

Create a new image based on the ubuntu image + the changes made:
```
sudo docker commit <idofcontainerlistedinthelastoutput> <yourfirstname>/nginx
```

Remove the container just run now that we've created an image based on it:
```
sudo docker rm <idofcontainerlistedinthelastoutput>
```

Run a new container based on the image created above and start the nginx service in the background:

```
sudo docker run -d -p 80:80 --net=host <yourfirstname>/nginx nginx -g 'daemon off;'
```

Note the -d switch which runs the container in the background (i.e. you'll remain at the prompt provided by your 'Docker VM' after seeing an ID printed to screen which is the new container's ID).

On 'Mgmt VM':
Browse to http://<dockervmeth0ipadress>

You should see the page that you edited.

Note. The new image is currently only stored locally as it's not been pushed to a registry such as Docker Hub or Azure Container Registry.  This would mean a lack of 'portability'.

Clear up:
```
sudo docker ps
```
```
sudo docker stop <idofcontainerlistedinthelastoutput>
```
```
sudo docker rm <idofcontainerlistedinthelastoutput>
```
<br><br>

## Step 8 - Inspect the layers of our image:

```
sudo docker history <yourfirstname>/nginx
```



<i>Declaratively template</i> the process above using a 'Dockerfile'

Run a container based on the newly created image (currently only stored locally)

Declaratively template a multi-container template using Docker Compose

Run an environment based on the template

