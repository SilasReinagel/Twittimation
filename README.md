# Twittimation &middot; [![Build status](https://ci.appveyor.com/api/projects/status/u80qht76djxs66a5/branch/master?svg=true)](https://ci.appveyor.com/project/TheoConfidor/twittimation/branch/master) [![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg)](./LICENSE) 

Twittimation is a .NET Core 2.0 Console App designed to allow you to schedule tweets, and automate Twitter interactions.

<img src="./logo.png" alt="Twittimation Logo" width="320"/>

----

## Features

1. Tweet from the Command Line
2. Schedule Tweets in Advance
3. Automatically Like a Percentage of Followee's Tweets

----

## Build

1. Install the [.NET Core SDK 2.0+](https://dotnet.microsoft.com/download)
2. Clone this repository `git clone https://github.com/SilasReinagel/Twittimation/`
3. Navigate to the repository folder and run `dotnet build`

----

## Installation

After Building the project:

1. `dotnet publish` targetting your [system's runtime](https://docs.microsoft.com/en-us/dotnet/core/rid-catalog)
2. Copy the publish output to the installation folder of your choice

----

## Run

1. Setup an App for your Account on the [Twitter Developer Portal](http://developer.twitter.com/)
2. Save your Consumer and Access Keys and Secrets using the `savecredentials` command
3. Run the `help` command to view the other application commands

----

## License

You may use this code in part or in full however you wish.  
No credit or attachments are required.
