# A simple introduction to Docker {using Docker Machine}

<br><br><br>

## Step 1 - Get access to a Docker host.

This has been completed in advance of today's session using the Docker Machine tool.  You can access your Docker VM by entering 'docker' commands as you normally would in a terminal when the engine is installed-and-running locally.

FYI:
<br>
[Install Docker CE on a Linux OS](https://docs.docker.com/install/)
<br>
[Install Docker for Windows](https://docs.docker.com/docker-for-windows/install/)
<br>
[Deploy a remote Linux Docker VM using Docker Machine](https://docs.docker.com/machine/)

<br>

Before moving on to Step 2, please make a note of the Azure Public IP Address that your Docker Machine VM has associated with it.  This can be found by entering the following commands or by having a look using Azure Portal:

```
docker-machine active
```

Copy the machine name from the output and then...

```
docker-machine ip <dockermachinename>
```

You'll need that IP address later.

<br><br>

## Step 2 - Run a container using the 'Hello World' image.

View your empty local image repository before you enter any other commands:

```
docker images
```

<br>

Now, run your first container:

```
docker run hello-world
```

<br>

Note the "Unable to find image 'hello-world:latest' locally" and the subsequent "Pulling" message.

<br>

You should see something like:

"Hello from Docker!
This message shows that your installation appears to be working correctly.

To generate this message, Docker took the following steps:
 1. The Docker client contacted the Docker daemon.
 2. The Docker daemon pulled the "hello-world" image from the Docker Hub.
    (amd64)
 3. The Docker daemon created a new container from that image which runs the
    executable that produces the output you are currently reading.
 4. The Docker daemon streamed that output to the Docker client, which sent it
    to your terminal.

To try something more ambitious, you can run an Ubuntu container with:
 $ docker run -it ubuntu bash

Share images, automate workflows, and more with a free Docker ID:
 https://hub.docker.com/

For more examples and ideas, visit:
 https://docs.docker.com/engine/userguide/"

<br>

Two things have happened:
1) An image has been downloaded from Docker's public 'Docker Hub' registry/repository and stored in your local host's image repository.
2) A container has been run atop the image and, because of the autorun script stored within the image, it has started, printed information to the screen/log, and then exited.

<br>

You can validate this by checking your local image repository again and by checking that a container has run and then exited.

Enter the following command again:

```
docker images
```

You should now see a 'hello-world' image in your repository and it will have a 'latest' tag.   We'll get into image tagging later on.

<br>

Enter the following command to see all of the containers that have ever run or are actively running on your docker host:

```
docker ps -a
```

<br>

Notice under 'COMMAND' the command that was run.

<br>

Note. The following command shows only running containers:

```
docker ps
```

<br><br>

## Step 3 - Pull another image from Docker Hub, inspect it, and then run a container that doesn't instantly exit.

<br>

Let's avoid doing anything fancy for now.  We'll start with a base OS image:

```
docker pull ubuntu:latest
```

<br>

Docker images are particularly interesting.  They're not like virtual machine images/disks at all.  They're actually made up of file system layers.  In order to see an example of how image layers are added, enter this command to view the layers of the official Ubuntu image that's now stored in your Docker Host's local repository:

```
docker history ubuntu
```

When you run a container, it writes to a {thin} writable layer at the top and all of the layers below are readable layers.  We will work with a 'Dockerfile' in the last step but that's one way that layers are added.   A Dockerfile is a top-down list of instructions and each line that causes a change adds a layer.

More on images, containers, and storage drivers can be found here: https://docs.docker.com/v17.09/engine/userguide/storagedriver/imagesandcontainers/ 

<br>

Next, run a container atop the new image and attach to the container rather than leaving it running in the background:

```
docker run -it ubuntu:latest /bin/bash
```

<br>

If you've been on a Linux OS before, things will be pretty familiar.  Enter a few commands like 'pwd', 'ls', 'touch newfile', etc.

<br>

Let's now take a look at how the container is connected to the network because this is important to understand; especially when considering how to scale beyond a single Docker host and also when looking at Kubernetes Services and Service Discovery later today.

Enter the following commands:

```
apt update
apt install -y net-tools
```

```
ifconfig -a
```

<br>

Every single container on every single Docker Host used today will have an 'eth0' interface IP address of 172.17.0.x within the network 172.17.0.0/16.  The most probable IP address that you're looking at on this container is 172.17.0.2 (i.e. the first one that's available).

This is because every newly-provisioned Docker Host will - by default - create a local virtual bridge with the same IP subnet assigned.  This means that it's a private LAN for only the containers.  In order to get access to the container from 'the outside' a NAT to the Docker Host's real IP address is needed and, when that is setup, each port can only be used once (i.e. there can only be one web server on TCP80 and/or TCP443).   We'll quickly look at that now.  Exit the container:

```
exit
```

or use the shortcut key:
<br>

'Ctrl+d'

<br>

See that the container stops at that point rather remaining in a running state (under 'STATUS'):

```
docker ps -a
```

<br>

To execute a command on your Docker Host, enter the following to connect to it over SSH:

```
docker-machine ssh <machinename>
```

<br>

Now look at the Docker Host's network interfaces:

```
ifconfig -a
```

"docker0" is the local virtual bridge.  "eth0" is the Docker Host's real interface.

<br>

Disconnect the SSH session ("exit") and do a quick clean-up to finish 'Step 3'.

<br>

Clean-up by entering the following command for each of the container IDs shown in your 'docker ps -a' output:

```
docker rm <containerid>
```

<br><br>

## Step 4 - Run a container atop the image pulled down in Step 3, attach, install the dotnetcore sdk, create a web app, run and test the web app, and exit.

<br>

Tip.  You may need to right-click the cmd terminal window for 'Edit --> Paste' during this step.

Note. You would not build a 'dotnetcore' image like we're going to as Microsoft maintains images and it doesn't make sense to have the sdk in every image if the app has already been built/code has been compiled.  See: https://hub.docker.com/r/microsoft/dotnet/ for information.  The image that we're about to build also ends up being very large (i.e. lots of data to push and pull to-and-from an image registry/repository - not very efficient).

<br>

The first thing to do in this step is to start a new container atop the Ubuntu image:

```
docker run -it -p 80:5000 ubuntu:latest
```

You may have noticed a subtle change in the command above.  We now have "-p 80:5000" added.  This will setup a NAT-P from our container's TCP port 5000 to TCP port 80 on the Docker Host's real interface thus making the container accessible from the Windows VM that you're currently RDP'd into {or, indeed, anywhere on the internet}. 

<br>

Once the container is running and you're attached to it, the following commands need entering to install dotnetcore sdk dependencies and then to install the dotnetcore sdk:

```
apt-get update && apt-get install -y --no-install-recommends ca-certificates curl wget nano && rm -rf /var/lib/apt/lists/*
```

```
apt-get update && apt-get install -y --no-install-recommends bzr git mercurial openssh-client subversion procps && rm -rf /var/lib/apt/lists/*
```

```
apt-get update && apt-get install -y --no-install-recommends libc6 libcurl4 libgcc1 libgssapi-krb5-2 libicu60 liblttng-ust0 libssl1.0.0 libstdc++6 libunwind8 libuuid1 zlib1g && rm -rf /var/lib/apt/lists/*
```

```
curl -SL https://dotnetcli.blob.core.windows.net/dotnet/Sdk/2.1.301/dotnet-sdk-2.1.301-linux-x64.tar.gz --output dotnet.tar.gz && echo "2101df5b1ca8a4a67f239c65080112a69fb2b48c1a121f293bfb18be9928f7cfbf2d38ed720cbf39c9c04734f505c360bb2835fa5f6200e4d763bd77b47027da dotnet.tar.gz" | sha512sum -c - && mkdir -p /usr/share/dotnet && tar -zxf dotnet.tar.gz -C /usr/share/dotnet && rm dotnet.tar.gz && ln -s /usr/share/dotnet/dotnet /usr/bin/dotnet
```

```
DOTNET_RUNNING_IN_CONTAINER=true
DOTNET_USE_POLLING_FILE_WATCHER=true
NUGET_XMLDOC_MODE=skip
mkdir warmup && cd warmup && dotnet new && cd .. && rm -rf warmup && rm -rf /tmp/NuGetScratch
```

<br>

The dotnetcore sdk is now installed within the container so you can now create a basic Razor Pages web app:

```
dotnet new webApp -o mockapp1
```

<br>

Before running 'dotnet run' to host your basic app on a local Kestrel server, a setting needs changing in order to allow access from outside the local machine.  Start by entering the following:

```
cd mockapp1/Properties/
nano launchSettings.json
```

Then change the "applicationUrl": "https://localhost:5001;http://localhost:5000" text to  "applicationUrl": "http://0.0.0.0:5000" and enter Ctrl+o followed by Ctrl+x to save the file and exit nano.

<br>

The 'dotnet run' command can now be entered to run and test access to the application:

```
cd ..
dotnet run
```

<br>

Grab the Docker Host's Public IP Address that you copied earlier and use a web browser to hit it over http.

You should see a basic Razor Pages Web App.

You should also see some logs on your cmd terminal when you return to it.

Now, exit the container by entering Ctrl+c folllowed by "exit".  In the next step you will convert the stopped container into an image which will mean that your application and all of its dependencies will be in one compact, portable, and redeployable package... i.e. you'll start to see the real value of Docker!

Do not remove the container that has just exited.

<br><br>

## Step 5 - Build a new image based on the addition of a new layer that was added in the last step atop the original 'Ubuntu' image.

<br>

The container that you were just working with is stopped.  The changes that you made were all written to a thin writeable layer and, as you haven't removed the container yet, the layer is still stored locally.  You're now going to 'commit' the container/layer - which includes your new application and its dependencies - to a new image.  Start by copying the ID of the stopped container from the output of:

```
docker ps -a
```

<br>

Now commit to an image called 'localrepo/mockapp1':

```
docker commit <containerid> localrepo/mockapp1
```

<br>

Inspect the output of:

```
docker images
```

The new image that you see can be shipped around by pushing it to a cental image registry/repository followed by pulling it down on any Linux Docker Host.  This includes a Raspberry Pi, a Kubernetes Node/Minion, a Linux VM under Azure's Web App for Containers service, etc. and the application's code and runtime environment inc. dependencies will be identical in all of those places. 

<br>

We'll keep things local for now though.  Run a container atop your new image using:

```
docker run -d -p 80:5000 localrepo/mockapp1:latest dotnet run --project /mockapp1
```

Notes. The '-d' switch runs the container in the background (hint. Kubernetes will run all containers with this switch).  We're also executing the 'dotnet run --project /mockapp1' when the container starts because we didn't setup a startup script within the container that we worked on.

<br>

Grab the Docker Host Public IP address that you copied earlier and use a web browser to hit it over http.

Just like before, you should see a basic Razor Pages Web App.

<br>

Enter these commands if you would like to see the container's logs:

```
docker ps
```

Copy the container's ID...

```
docker logs <containerid>
```

<br>

It's also worth checking out 'docker exec':

```
docker exec <containerid> ps -ef
```

<br><br>

## Step 6 - Template an image build using a 'Dockerfile', create an Azure Container Registry, and push the new image up to it.

<br>

Open Visual Studio Code and create a new file.

Paste the following text into the file, have a look over what the lines imply will happen at each step of the build, and then save the file with a name of 'Dockerfile' in a folder called 'nginx' under 'C:\Docker\'.

```
FROM ubuntu

RUN apt-get update

RUN apt-get install -y nginx

RUN echo "\ndaemon off;" >> /etc/nginx/nginx.conf

CMD ["nginx"]

EXPOSE 80
```

<br>

Create a new Azure Container Registry.  This can be done using Azure CLI.

Go to your open cmd terminal and login to Azure:

```
az login
```

A web browser tab will open and after you authenticate you should see a print of some json that describes the subscriptions that you have access to.  Select the correct Subscription (i.e. 'Development'):

```
az account set --subscription <subscriptionid>
```

<br>

Create a new Azure Container Registry:

```
az acr create --resource-group <resourcegroupname> --name <containerregistryname> --sku Basic
```

Note. 'resourcegroupname' = sd-we-con-dev-rg'LabID'  and  'containerregistryname' = sdwecondevacr'LabID'

<br>

Now, login to your Azure Container Registry by first getting your credentials:

```
az acr credential show --name <containerregistryname>
```

Followed by logging in with Docker:

```
docker login <containerregistryname>.azurecr.io --username <containerregistryname> --password <eitherpasswordshowninlastoutput>
```

<br>

Navigate to the location of the Dockerfile file in your cmd terminal and perform an image build using the Dockerfile just created by entering:

```
docker build -t <containerregistryname>.azurecr.io/nginx .
```

<br>

Run a container using the new image:

```
docker run -d -p 80:80 <containerregistryname>.azurecr.io/nginx
```

<br>

Test by browsing to your Docker Host's Public IP Address (as before).

You should see a page hosted on the nginx web server saying "Welcome to nginx!".

<br>

Stop the container and remove it:

```
docker ps
```
...

```
docker stop <containerid>
```

```
docker rm <containerid>
```

<br>

The next thing to do is to push your new nginx image to your container registry.  You're already logged into your Azure Container Registry so it's as simple as:

```
docker push <containerregistryname>.azurecr.io/nginx
```

<br>

Once the upload is finished, take a look at your container registry:

```
az acr repository list --name <containerregistryname> --output table
```

<br>

The 'nginx' image shown can now be pulled down by any authenticated Docker Host.

<br>

To test this, remove your local image, view your local image repository, pull the image down from your Azure Container Registry, and then view your local image repository again:

```
docker images
```

```
docker rmi <containerregistryname>.azurecr.io/nginx
```

```
docker images
```

```
docker pull <containerregistryname>.azurecr.io/nginx
```

```
docker images
```

<br>


END