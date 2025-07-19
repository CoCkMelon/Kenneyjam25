% 3x3 grid puzzle in MiniZinc
% Each cell can be 0 or 1 (off/on)

% Decision variables: 3x3 grid
array[1..3, 1..3] of var 0..1: grid;

% Constraint 1: Each row must have exactly 2 active buttons
constraint forall(i in 1..3) (
    sum(j in 1..3) (grid[i,j]) = 2
);

% Constraint 2: Each column must have exactly 2 active buttons
constraint forall(j in 1..3) (
    sum(i in 1..3) (grid[i,j]) = 2
);

% Constraint 3: At least one corner must be active
constraint (grid[1,1] + grid[1,3] + grid[3,1] + grid[3,3]) >= 1;

% Solve
solve satisfy;

% Output
output [
    if j = 1 then "\n" else " " endif ++
    show(grid[i,j])
    | i in 1..3, j in 1..3
] ++ ["\n"];
