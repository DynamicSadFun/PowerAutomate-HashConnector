# PowerAutomate-HashConnector

Cryptographic hash functions are an indispensable and ubiquitous tool used to perform a variety of tasks, including authentication, data integrity checking, protecting files, passwords, and even malware detection.
For several years, the application for this functionality lay on the powerapps-community as an idea:

https://powerusers.microsoft.com/t5/Power-Apps-Ideas/Function-to-encrypt-hash-sha512-sha1-sha256-MD5/idi-p/449377

Most likely, this problem has been solved privately many times, but no one has issued a public solution.
I really hope that the functionality I suggested solves this problem.
Azure Function was created that accepts get-post requests as input. We can pass it the hashing type (md5/sha256/sha512/rsa) and any message to the input, starting from text and ending with binary data (pictures, movies etc.), and at the output we get a string of fixed length.
The code is written on C# language and can be easily improved. If you are missing encryption methods, just add them to this code similarly to the existing ones using the System.Security.Cryptography library.
Further, this code must be published in your Cloud as an Azure Function. The instructions for publishing are here:

https://docs.microsoft.com/en-us/azure/azure-functions/functions-create-your-first-function-visual-studio

Immediately after the publication of the function, it can already be used through the HTTP:

![image](https://user-images.githubusercontent.com/86048404/125761379-5e2abd3c-c281-4b0b-8f91-9f873f4fca68.png)

But for Power Automate, this is not enough. We need to create a custom connector that will perform the required transformations. The instructions for creating a connector are here, but you don't need to follow most of these steps: 

https://docs.microsoft.com/en-us/connectors/custom-connectors/define-blank#:~:text=Power%20Apps-,Start%20the%20custom%20connector%20wizard,custom%20connector%2C%20then%20select%20Continue.

Take the code of my connector (swagger.json) and publish it.
In general, that's all. Use this functionality in your Power Automate to encrypt messages and files, store the hashes as you see fit:

![image](https://user-images.githubusercontent.com/86048404/125761522-9af8fe86-8036-4085-8068-3f7e676400ee.png)

As you can see, the functionality is very simple, but quite effective. Of course, there are nuances (cons) in it. Unfortunately I am not an expert in Power Automate (yet), and therefore your comments and  suggestions will be very useful.
Good luck with your use!
