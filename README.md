# PowerAutomate-HashConnector

![image](https://user-images.githubusercontent.com/86048404/126144714-2ee0eeca-eb28-4672-94e2-7a3d29d70f08.png)

Cryptographic hash functions are an indispensable and ubiquitous tool used to perform a variety of tasks, including authentication, data integrity checking, protecting files, passwords, and even malware detection.
For several years, the application for this functionality lay on the powerapps-community as an idea:

https://powerusers.microsoft.com/t5/Power-Apps-Ideas/Function-to-encrypt-hash-sha512-sha1-sha256-MD5/idi-p/449377

Most likely, this problem has been solved privately many times, but no one has issued a public solution.
I really hope that the functionality I suggested solves this problem.
Azure Function was created that accepts get-post requests as input. We can pass it the hashing type (md5/sha256/sha512/rsa) and any message to the input, starting from text and ending with binary data (pictures, movies etc.), and at the output we get a string of fixed length.

**Information for developers:**
The code is written on C# language and can be easily improved. If you are missing encryption methods, just add them to this code similarly to the existing ones using the System.Security.Cryptography library.
Further, this code must be published in your Cloud as an Azure Function. The instructions about create new project and for publishing are here:

https://docs.microsoft.com/en-us/azure/azure-functions/functions-create-your-first-function-visual-studio

**Information for consultants:**
Open the Result directory, grab the zip file from there and publish it to your cloud as follows:
You can use Azure CLI to trigger a push deployment. Push deploy a .zip file to your function app by using the az functionapp deployment source config-zip command. To use this command, you must use Azure CLI version 2.0.21 or later. To see what Azure CLI version you are using, use the az --version command.

In the following command, replace the <zip_file_path> placeholder with the path to the location of your .zip file. Also, replace <app_name> with the unique name of your function app and replace <resource_group> with the name of your resource group.
```powershell
az functionapp deployment source config-zip -g <resource_group> -n \
<app_name> --src <zip_file_path>
```
This command deploys project files from the downloaded .zip file to your function app in Azure. It then restarts the app. To view the list of deployments for this function app, you must use the REST APIs.

When you're using Azure CLI on your local computer, <zip_file_path> is the path to the .zip file on your computer. You can also run Azure CLI in Azure Cloud Shell. When you use Cloud Shell, you must first upload your deployment .zip file to the Azure Files account that's associated with your Cloud Shell. In that case, <zip_file_path> is the storage location that your Cloud Shell account uses. For more information, see this link:

https://docs.microsoft.com/en-us/azure/azure-functions/deployment-zip-push#deployment-zip-file-requirements

Immediately after the publication of the function, it can already be used through the HTTP:

![image](https://user-images.githubusercontent.com/86048404/126145124-9f5bd509-932f-4641-91e1-e3532364b7a2.png)

But for Power Automate, this is not enough. We need to create a custom connector that will perform the required transformations. The instructions for creating a connector are here, but you don't need to follow most of these steps: 

https://docs.microsoft.com/en-us/connectors/custom-connectors/define-blank#:~:text=Power%20Apps-,Start%20the%20custom%20connector%20wizard,custom%20connector%2C%20then%20select%20Continue.

Take the code of my connector (swagger.json), change host name:
```json
 "swagger": "2.0",
  "info": {
    "title": "Logiq Apps Hash Connector",
    "description": "",
    "version": "1.1"
  },
  "host": "functionhashbylogiqapps.azurewebsites.net",
  "basePath": "/api/"
```
and publish it. 
In general, that's all. Use this functionality in your Power Automate to encrypt messages and files, store the hashes as you see fit:

![image](https://user-images.githubusercontent.com/86048404/126145202-4bfd3997-0685-49d0-9d36-5473cf655b2d.png)

As you can see, the functionality is very simple, but quite effective. Of course, there are nuances (cons) in it. Unfortunately I am not an expert in Power Automate (yet), and therefore your comments and  suggestions will be very useful.
Good luck with your use!
