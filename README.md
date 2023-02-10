# bimU x Tekla - Rebar Inspection
A web-based building information model (BIM) viewer for safety training on construction site. The viewer is powered by bimU Viewer APIs.

[bimU Tekla Rebar Inspection](https://user-images.githubusercontent.com/119405090/218068591-c6a1fd64-6867-47d1-ade9-7bc17ec9ae57.mp4)

## Features
- Easily switch to your own BIM models
- Provide images and specification for each critical area
- Highlight BIM elements with animation for each critical area

## Live Demo
Main branch is deployed to https://structural-inspection.netlify.app/.

## Project Setup

### Rules for Element Properties
1. 4 custom fields for all elements in BIM models including 勞安_法規內容, 勞安_法規編號, 勞安_法規圖片, 勞安_法規影片.
2. Seperate by '@' for multiple rules in the same element (applied to all fields).

![模型元件填入資料說明](https://user-images.githubusercontent.com/119405090/218041642-e19c1a92-b64b-4db2-adf9-91e41863ae35.png)

### Prerequisites
- BIM model uploaded to [bimU Viewer](https://viewer.bimu.io)
- `model id` from bimU Viewer

### Setup
- clone this repository
- install dependencies: `npm install`
- run server: `npm start`
- go to http://localhost:3000
    - add url parameter `modelId` to specify your own model
    - add url parameter `imgFolder` for the name of image folder accommdated the training materials (same as the name in the public folder of repo)

Enjoy :metal:
