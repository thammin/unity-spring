# A minimal spring physics library for Unity

Implement multiple solvers for damped harmonic oscillator.

Solvers:

-   [x] [Closed-form solution for the ODE](http://www.ryanjuckett.com/programming/damped-springs/)
-   [x] [Semi-implicit Euler method](https://en.wikipedia.org/wiki/Semi-implicit_Euler_method)

Maybe:

-   [ ] [Runge-Kutta 4th order aka RK4](https://en.wikipedia.org/wiki/Runge%E2%80%93Kutta_methods)
-   [ ] [Verlet Integration](https://en.wikipedia.org/wiki/Verlet_integration)

Maybe not:

-   [ ] [Explicit Euler aka Forward Euler](https://en.wikipedia.org/wiki/Euler_method)
-   [ ] [Implicit Euler aka Backward Euler](https://en.wikipedia.org/wiki/Backward_Euler_method)
-   [ ] [Mid-point method](https://en.wikipedia.org/wiki/Midpoint_method)

# Install

# Usage

# Screenshot or demo

Closed-form solution:
![](./Documentation~/closed-form.png)

# Faq

## Unity SmoothDamp

Source code: [link](https://github.com/Unity-Technologies/UnityCsReference/blob/2019.3/Runtime/Export/Math/Mathf.cs#L302-L331)

Based on closed-form solution, but only modeling critically damped spring only. Using tweaked Exponential approximation (up to Taylor 3rd order) which claims as roughly 80 times faster and approximate less than 0.1% error than `exp` function.

<img src="./taylor-exp.svg">

```cs
// tweaked coefficients
float exp = 1F / (1F + x + 0.48F * x * x + 0.235F * x * x * x);
```

# License

MIT
