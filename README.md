# FolderSyncronization

## Table of Contents

- [Introduction](#introduction)
- [Features](#features)
- [Installation](#installation)
- [Usage](#usage)

## Introduction

This program synchronizes two folders: a source folder and a replica folder, ensuring the replica folder maintains an identical copy of the source folder.

## Features

- Periodic one-way synchronization from source to replica folder.
- Logging of file operations to a specified log file and console output.

## Prerequisites

- .NET Core SDK.
- .NET 8.0

## Installation

Simply Extract the zip file from Github releases, and navigate to the net8.0 folder 

### Usage

```bash=
.\FolderSycronization -s <source_folder> -r <replica_folder> -i <interval> -o <output_log>

  -s, --source      Required. Source path folder to be replicated

  -r, --replica     Required. Path to replicate source folder

  -i, --interval    Required. Interval to syncronize, interval in ms

  -o, --output      Required. Output file for logs

  --help            Display help screen.
```

#### Usage Notes:

If the path has whitespaces please encapsulate the path with quotes  like so "C:/Desktop Env/source".

After executing the program, simply Ctrl-C to terminate it. It might take some time since it awaits to finish the current syncronization due to file integrity issues.
