# PreDeploymentUtility
##Windows Desktop Application

The Pre-deployment Utility can be used to capture network information about a user and display it or save it to a file. You can also use it to transfer files from a user’s local directory to a remote network drive.

It is intended to save you time when migrating a user from one computer to another as part of your Pre-Deployment process. This utility can be useful for anyone whose job include migrating user's computers like a Computer Repair Shop or an IT department.

It is far from complete and it is a work in progress. The UI is not yet like the final version. See http://hsnyc.co/portfolio/pre-deployment-utility/ for the desire UI. 

I work on it as time allows and I welcome your help in either the UI or the code.

Feel free to fork it, share it, or use it for your own projects.  Please reach out if you have any questions.

Thanks for your interest in this project.

###To get started using it right away..
* Download the project files and open it with any recent version of Visual Studio.
* The Visual Studio Community version is freely available from Microsoft at https://www.visualstudio.com/free
* In its current state, the app will list the user's info but to have it backup the files you will need to edit the path in line 246 in the  *MainWindow.xaml.cs* file.
* So say that in your company users are scripted to map to the S:\ drive for their Home directory, then change this to @"S:\" 
