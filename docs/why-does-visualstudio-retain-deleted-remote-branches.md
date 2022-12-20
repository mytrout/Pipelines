# Why does Visual Studio 20xx show deleted remote branches in the UI?

There is a command line function in Git to prune remote branches that have been deleted
git remote prune origin

Why should this capability require me to do ANYTHING?

## Stack Overflow to the rescue....again!

There is a setting in Visual Studio that will remove deleted remote branches.

It is located under Git...Settings...Git Global Settings...Prun remote branches during fetch.

By default the value is set to "Unset".

Once it is changed to True and a fetch is performed, all of the origin remote branches that have been deleted will disappear from the UI.

https://stackoverflow.com/questions/72819125/visual-studio-git-remote-list-still-shows-deleted-branches