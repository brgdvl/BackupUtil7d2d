# BackupUtil7d2d
Backup tool for 7d2d server

This tool is ment to be a utility that enables backup of saved files on server, to local.

Project is just made as some sort of training/hobby project. The Principle of SOLID has not been followed to a full extent, likewise other software development techniques.

Prerequirement:
* create a config.json file in folder configs, at same level as compiled Assembly.
  - ./BackupUtil7d2d.DLL
    - |-configs
      - |-config.json

* config.json is formatted as follow
```
  {
  sftp:{
    "host":"",
    "port":"",
    "username":"",
    "password":"",
    "remotedir":"",
    "savedir":"",
    "localbackupdir":""
    }
  }
  ```
  
** /\\/\\/\\/\\/\\/\\/\\/\\/\\/\\/\\/\\/\\/\\/\\/\\/\\/\\/\\/\\/\\/\\/\\/\\/\\/\\/\\/\\/\\/\\/\\/\\/\\/\\/\\/\\/\\/\\/\\/\\/\\/\\/\\/\\/\\/\\

TODO:
  * add threading
  * decouple config file (remove hardcoding)
