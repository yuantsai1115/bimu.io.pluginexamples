# bimU x Tekla - Rebar Inspection (Experimental:bulb:)
A bimU Agile plugin for automatically extracting rebar information and creating issues for rebar inspection. This is one of the experimental plugins for the dedicated scenario.

[bimU Tekla Rebar Inspection](https://user-images.githubusercontent.com/119405090/218068591-c6a1fd64-6867-47d1-ade9-7bc17ec9ae57.mp4)

## Background
Quality check for rebar is regularly conducted during the construction. Conventionally, inspectors bring drawings and inspection sheets (paper-based) to the site and check whether the construction matches the design. This plugin targets to improve the process in three aspects:
1. **Speed up the preparation for onsite inspection**: Design data of rebar can be ***extracted and listed automatically***.
2. **Digitalised management of inspection**: Only an iPad or tablet is needed onsite to ***view the design data***, ***take photos***, and ***fill in the inspection sheet***.
3. **Speed up the documentation process**: ***Reduce time*** for creating inspection report after the inspection.

<img src="https://user-images.githubusercontent.com/119405090/218373931-59e2f7ed-08ea-4552-9a61-914b271ce80d.jpg" height="300">

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
