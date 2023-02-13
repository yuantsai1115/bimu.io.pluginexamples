# bimU x Tekla - Rebar Inspection (Experimental:bulb:)
A bimU Agile plugin for automatically extracting rebar information and creating issues for rebar inspection. This is one of the experimental plugins for dedicated scenarios.

[bimU Tekla Rebar Inspection](https://user-images.githubusercontent.com/119405090/218068591-c6a1fd64-6867-47d1-ade9-7bc17ec9ae57.mp4)

## Features
- Extract rebar design data from Tekla BIM model
- Batch create inspection items as issues
- Easily manage and review issues through bimU Agile table view and kanban view
- Export inspection report with customisable Word template

| <img src="https://user-images.githubusercontent.com/119405090/218071602-f71b1f63-53b6-43b5-9ffb-1d6379a4454c.png" width="600"> | 
|:--:| 
| *bimU Agile Table View* |

| <img src="https://user-images.githubusercontent.com/119405090/218071642-edea991d-0a7f-4b36-8678-dc36a571635e.png" width="600"> | 
|:--:| 
| *bimU Agile Kanban View* |

## Live Demo
Main branch is deployed to https://structural-inspection.netlify.app/.

## Project Setup
### Prerequisites
- Account for [bimU Agile](https://bimu.io) (Free to registered)
- Get a Tekla BIM rebar model
- Download & Install [bimU Launcher](https://docs.bimu.io/viewer/upload-a-bim-model/#install-bimuio-launcher)

### Setup
- Clone this repository
- Compile the source code in Visual Studio
- Copy & Paste the `.dll` file to `C:\ProgramData\bimU.io\plugins`
- Click `Import issues from plugin` button on bimU Agile to execute the plugin 

    <img src="https://user-images.githubusercontent.com/119405090/218370799-d5721e93-7b19-43e1-a936-c1949d383021.jpg" width="400">

Enjoy :metal:
