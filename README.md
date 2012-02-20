## Github For Outlook
Description  

The tool should give you an overview of the important things you need to address for the projects you look after:

* New and updated Pull Requests.
* New and updated Issues.
* Display notifications as configured on GitHub.
* View and respond to comments from inside Outlook.
* Leverage Outlook features like email, labels and perhaps tasks to integrate GitHub work with the rest of your day-to-day work.

###The technology under the hood

The most popular tool of building addins for Microsoft Office is called Visual Studio Tools for Office - which allows .NET apps to integrate and behave like native Office addins. There's a bunch of resources on getting started with VSTO over on MSDN.

We will be using WPF for the UI, but the real project we will leverage is Jake Ginnivan's VSTOContrib libraries - this will allow us to quickly scaffold the code without getting bogged down in COM interop woes.

We're also talking with @aeoth about making use of some of his recent work on a desktop port of the "Milestone" GitHub client for WP7. This isn't cruical to the end goal of the week, but after using Reactive Extension in Carnac we're keen to make use of it in other scenarios.

### Getting started

**Getting started with Git and GitHub**

 * [Setting up Git for Windows and connecting to GitHub](http://help.github.com/win-set-up-git/)
 * [Forking a GitHub repository](http://help.github.com/fork-a-repo/)
 * [The simple gude to GIT guide](http://rogerdudler.github.com/git-guide/)

Once you're familiar with Git and GitHub, clone the repository and run the ```.\build.cmd``` script to compile the code and run all the unit tests. You can use this script to test your changes quickly.

### Discussing ideas 

* [Trello Board](https://trello.com/board/github-tasks-for-outlook/4f4234cdbfa22c0070ac4caa) - add ideas, or claim an idea and start working on it!
* [JabbR Chatroom](http://jabbr.net/#/rooms/code52) - discuss things in real-time with people all over the world!
