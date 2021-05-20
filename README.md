# GamesEngines2Assignment
 
##Video
[![Watch the video](https://img.youtube.com/vi/lgXfAAuksV0/maxresdefault.jpg)](https://youtu.be/lgXfAAuksV0)

##Story: 
For this project I want to showcase the flocking algorithm through an underwater scene.
I think this makes far better use of the AI techniques required from this project and will be visually more interesting.
I will be using a low poly asset pack for most of the environment and fish models.
The low poly design will help keep the scene running smoothly.

There are 1000 boids moving and interacting with each other and the environment in the scene.
Different flocks of fish, birds and sharks are used as well as singular boids like the air balloon and ship.

The story is driven by the interactions of the boids and cameras.
There are objects placed around the scene to give an element of environmental storytelling.

##Behaviour: 
  - Cohesion
  - Avoidance
  - Alignment
  - Obstacle Avoidance
  - Bounds

All behaviours are running simultaneously with different weightings to affect the overall outcome.
Doing so removed the need for a behaviour tree.
The base behaviours were learned from the GameDevChefs video on flocking AI and altered to fit my requirements.

##Assets:
Taken from the Low Poly Ultimate Pack on the unity asset store (https://assetstore.unity.com/packages/3d/props/low-poly-ultimate-pack-54733).
The low poly models maked to look I wanted.

##Cameras:
  - Rotating camera above water
  - Rotating camera below water
  - 6 stationery cameras
  - Fish cam (View form fish as part of a flock)
  - Free roam player cam
  - Auto mode (Switches between all cameras except player on a set interval)
  
##How it works:
There are around 1000 moving boids in the scene running at 40 - 50 Fps. I tried adding more but could not optimise it any further to sustain a high enough frame rate.
The boids are split into different categories:
######Flock Idle:
This flock is set to freely move within the bounds of its set location.
######Flock Random Moving:
The target point of the flock is set to move to a new location within line of sight at certain intervals. This was set to trigger based change before but I prefered on a timer as it allowed the flock to reach its destination and mimic the idle state at a much larger scale for a moment.
######Flock Follow:
A single movement point the flock is constantly chasing.
######Singular 3D Moving:
Single entity moving around a 3D space eg. Hot air balloon/ sharks at sunken ship
######Singular 2D Moving:
Same as above but with the Y axis locks eg. Boat

##How to play:
When played it begins on auto mode switching between multiple cameras on a coroutine.
To switch between cameras manually press Tab to select the camera.
To move the free roam player camera, use wasd and mouse to look around.

##Other:
Small amount of post processing used to add bloom and a vignette.
Default unity fog used as well.
