# How did I get Github Actions, .NET 5.0, SonarCloud.io, and Code Coverage to work together?

I didn't do this remotely on my own.

1. This StackOverflow question led me to the breakthrough of installing the SonarQube .NET Scanner as a Global tool.
https://stackoverflow.com/questions/67597824/sonarscanner-for-net-fails-in-github-actions-net-5

2. After testing and re-testing and fiddling and frustrating, this article outlined the whole solution I used, but not the exact way I used it.
https://www.seeleycoder.com/blog/sonarqube-with-github-actions-and-net-core-5-x/

## Requirements
My requirements were to create a GitHub actions yaml file that would 
1. build my code
2. run the tests
3. collect code coverage.
4. publish SonarCloud results.
5. publish code coverage results to SonarCloud.
6. easily copied and altered for other parts of the Pipelines eco-system.
7. run on Linux.

## Accomplishment #1 - Running a Github Action for .NET 5.0
1, 2, and 7 were easily accomplished out of the gate with the example from Github located at:
https://docs.github.com/en/actions/guides/building-and-testing-net
Literally 30 minutes after reading the article on May 14th, 2021, these requirements were done.

## Accomplishment #2 - Publish results to SonarCloud
4 took a little tinkering, but I finally accomplished it around June 5 but tinkering with all of the settings.
https://stackoverflow.com/questions/67597824/sonarscanner-for-net-fails-in-github-actions-net-5
The above link finally got me the last steps.

## Accomplishment #3: Easily copied and altered
My discovery of Environment variables and secrets really cleaned everything in the bottom half of the yaml file up and pulled configurable items to the top.

## The Struggle is REAL - My Google-foo continued to fail me with attempting to use XPlat Code Coverage and Linux and Windows.
I tried running the actions on Windows but couldn't get a working copy with that configuration either.

## Accomplishmment #3 - Seely Coder to the rescue
After some really intense Google-foo, I ran across the following article published on May 27th (about a week before I started my odyssey)
https://www.seeleycoder.com/blog/sonarqube-with-github-actions-and-net-core-5-x/
After reviewing what was in the SonarQube.Analysis.xml file and the coverlet.runsetting file, I thought.....THIS IS IT!
Copying his information and converting those parameterized files into command line arguments, I was elated at the thought of a working Action with easy alterations without checking in additional files all over the project.

## FAILURE ONCE AGAIN ## - The parameter for the open cover was not properly formatted.
I ran a query in the coverlet-coverage github project to see why my parameter wasn't working
https://github.com/coverlet-coverage/coverlet/issues/984

## SUCCESS...FINALLY.
The "--DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=opencover" command line parameter was missing a space.
"-- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=opencover" and another build cycle later, Code Coverage metrics were showing up in SonarCloud.

All of my requirements had finally been met!
It only took me about a month to have enough free time to discover....what Jon Seely found out 30 days ago.

Enjoy the coffee, Jon.  I appreciate your efforts!

