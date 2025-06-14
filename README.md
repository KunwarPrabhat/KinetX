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

---

## 🛠️ Stuff I’m Working On

- **Gravity**
  - Apply a constant downward acceleration to simulate Earth-like gravity.
  - Add mass-based gravitational attraction between objects (Newton’s law of universal gravitation).
  - Include toggles for global vs. object-specific gravity.
  - Future support for custom gravity vectors and planetary simulation.

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

