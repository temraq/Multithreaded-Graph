# Multithreaded-Graph

This repository contains a program that implements graph algorithms using multithreading. The program utilizes two threads: a primary thread for overall program management and a secondary thread for performing computations.

Key features:

· Multithreading: The program uses two threads for efficient execution.
· Stage-based computations: The secondary thread performs computations in stages, transmitting results after each stage to the primary thread.
· Data sharing: The secondary thread writes results to a shared data area.
· Visualization: The primary thread reads results from the shared data area and visualizes them on the screen.
· Cycle finding:  The program finds a cycle that passes through no more than two central vertices of the graph.

Technologies:

· Assembly language:  The program is implemented in assembly language.
· Graphics library: The program uses a graphics library for visualization.
