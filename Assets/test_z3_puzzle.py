#!/usr/bin/env python3
from z3 import *

# Simple 3x3 grid puzzle
# Each cell can be 0 or 1 (off/on)
# Constraints:
# - Each row must have exactly 2 active buttons
# - Each column must have exactly 2 active buttons
# - Corner cells must be different from center

def solve_grid_puzzle():
    # Create 3x3 grid of boolean variables
    grid = [[Bool(f"cell_{i}_{j}") for j in range(3)] for i in range(3)]
    
    # Create solver
    s = Solver()
    
    # Constraint 1: Each row must have exactly 2 active buttons
    for i in range(3):
        row_sum = Sum([If(grid[i][j], 1, 0) for j in range(3)])
        s.add(row_sum == 2)
    
    # Constraint 2: Each column must have exactly 2 active buttons  
    for j in range(3):
        col_sum = Sum([If(grid[i][j], 1, 0) for i in range(3)])
        s.add(col_sum == 2)
    
    # Constraint 3: At least one corner must be active
    corners = [grid[0][0], grid[0][2], grid[2][0], grid[2][2]]
    corner_sum = Sum([If(corner, 1, 0) for corner in corners])
    s.add(corner_sum >= 1)
    
    # Solve
    if s.check() == sat:
        model = s.model()
        print("Solution found:")
        for i in range(3):
            row = []
            for j in range(3):
                value = model.evaluate(grid[i][j])
                row.append('1' if value else '0')
            print(' '.join(row))
        return True
    else:
        print("No solution found")
        return False

if __name__ == "__main__":
    solve_grid_puzzle()
