# 🎗️KinetX - Physics Engine :

So... I’ve been obsessed with physics since I was a kid and the more I read physics the clear I perceived the real world.  
Fast forward to engineering life: I kept asking myself *"how the hell do games make stuff bounce, roll, and explode so perfectly?"*  
and that made me curious to dive deeper into what's uneen.

So instead of googling the answer and moving on, I did something dumb (but awesome) —  
I decided to **build my own physics engine from scratch** in C#.
```
  Don't do it if you love living a peaceful life.
```
---

##  Why I Made This

- Because I wanted to understand the math and code that actually makes stuff work in a game.
- also because It comes up with a sense of satisfaction that I made something I was passionate about.
- And mainly... because why not?

This isn’t some industry-ready engine, It’s messy. 
But every line of code teaches me something I’ve always wanted to know.

---

##  Stuff I’ve Already Done

- **Build a UI good enough to interact**  
  _(PS - I hate it, eats up a lot of time)_

- **Circle-to-Circle Collision Detection**
  - Implements geometric detection of overlapping circles using distance checking between centers.
  - Uses a `Vector2D` structure for position and direction calculations.
  - Includes collision resolution using basic position correction based on penetration depth.

- **AABB (Axis-Aligned Bounding Box) Collision Detection**
  - Detects collisions between rectangular objects aligned with world axes.
  - Uses Impulse Resolution for Calculating normals and penetrationDepth for more realistic bounce off.

- **Circle to AABB Collision detection**
  - Implemented circle to AABB collision detection and impulse resolution as well.
  - Geometric Detection: Detects collisions between circular and axis-aligned rectangular objects.
  - Distance Calculation: Computes the shortest distance between the circle's center and the AABB's edges.
  - Collision Resolution: Implements impulse resolution to adjust velocities based on collision normals and penetration depths.
  - Code Example: Utilizes vector mathematics for precise collision detection and response.

- **Impulse Resolution**
  - Calculate collision impulse using the conservation of momentum and coefficient of restitution.
  - Adjust post-collision velocities based on object mass, velocity, and surface normal.
  - Incorporate restitution to control elasticity (perfectly elastic to inelastic collisions).

- **Movement Based on Velocity**
  - Applies linear velocity to each shape using the formula:  
    `position += velocity * deltaTime * rate`
  - Uses `Canvas.SetLeft()` and `Canvas.SetTop()` to update UI element positions in WPF.
  - Edge handling includes boundary detection and velocity inversion on hitting canvas edges.
  - Time-based updates ensure frame-rate independent motion for smoother simulation.

- **Gravity** 
  - Simulates real-world gravitational pull by applying a constant downward force to all objects.
  - Works seamlessly with Verlet integration for smooth motion without explicitly tracking velocity.
  - Gravity is applied per frame, affecting each object's position over time.
  - The system supports a large number of particles due to its simplicity and stability.
  - Fully integrated into the physics update cycle alongside constraint solving and collision detection.
  - Part of a modular 2D physics engine framework that also handles user-defined object parameters like mass and shape.
  - Supports dynamic adjustment of gravity direction or strength if extended.

-**Spatial Hashing Collision Detection**
  - Optimizes collision detection by drastically reducing the number of object pairs that need to be checked (from O(n²) to near O(n)).
  - The 2D space is divided into uniform grid cells based on object radius, and objects are hashed into these cells using their position.
  - Each object only checks for collisions against others in the same or neighboring cells, avoiding unnecessary comparisons.
  - Perfect for handling thousands of dynamic particles or objects with minimal CPU overhead.
  - Scales well with increasing object count; performance remains stable even with over 4000 objects.
  - Implemented with a dictionary-based spatial hash map for fast lookups and insertions.
  - Works seamlessly with circular collision bounds using a pre-defined `radius`.
  - Fully integrates with the engine’s physics loop and supports dynamic objects moving across cells.
  - Lays the groundwork for more advanced optimization like region activation or hierarchical grids.
  - Achieved 5000 + objects rendring at real time.
---
 
## 🛠️ Stuff to work on in future.

- **Fluid Simulation**
  - Implement a basic particle-based pseudo-fluid system.
  - Each particle simulates a droplet interacting with others via spring-like forces.
  - Maintain particle spacing to mimic soft-body or jelly-like behavior.
  - Suitable for basic fluid blobs, water surface simulation, or gelatinous interactions.

- **Something Crazy that might come to my mind**

##  Tools I'm Using

-  **Language**: C#
-  **Canvas**: WPF (Windows Presentation Foundation — sounds fancier than it is)
-  **Math**: My own `Vector2D` and collision structs for simlified calculations.
-   Not using any physics libraries or any game engine.



## 🤝 How to Contribute

Contributions are welcome! If you want to help improve the engine, add new simulations, or fix bugs, follow these steps:

1. **Fork the Repository**

Click the "Fork" button at the top right of this GitHub page.

2. **Clone Your Fork Locally**

    ```
    git clone https://github.com/<your-username>/KinetX.git
    cd KinetX
    ```
3. Make your Changes
   ```
   Add your code, test it locally, and make sure everything works.
   ```
4. Commit and Push.

5. Open Pull request.
   ```
   Go to your fork on GitHub and click "Compare & pull request". Describe what you did, and submit it.
   ```

