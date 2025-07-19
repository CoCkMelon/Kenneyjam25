% Power Flow Puzzle Model
% Grid with power sources, targets, and redirectors
% Goal: Route power beams to hit all targets

% Grid dimensions
int: rows = 5;
int: cols = 5;

% Element types
int: EMPTY = 0;
int: SOURCE = 1;
int: TARGET = 2;
int: REDIRECTOR = 3;

% Grid layout (fixed)
array[1..rows, 1..cols] of int: grid = [|
  SOURCE, EMPTY, EMPTY, EMPTY, TARGET |
  EMPTY, EMPTY, REDIRECTOR, EMPTY, EMPTY |
  EMPTY, REDIRECTOR, EMPTY, REDIRECTOR, EMPTY |
  EMPTY, EMPTY, REDIRECTOR, EMPTY, EMPTY |
  TARGET, EMPTY, EMPTY, EMPTY, SOURCE
|];

% Redirector angles (decision variables)
% 0=horizontal, 1=vertical, 2=diagonal/, 3=diagonal\
array[1..rows, 1..cols] of var 0..3: redirector_angle;

% Power beam paths (decision variables)
% 0=no beam, 1=beam horizontal, 2=beam vertical, 3=beam diagonal/, 4=beam diagonal\
array[1..rows, 1..cols] of var 0..4: beam_path;

% Target hit status (decision variables)
array[1..rows, 1..cols] of var 0..1: target_hit;

% Count sources and targets
int: num_sources = sum(i in 1..rows, j in 1..cols)(grid[i,j] = SOURCE);
int: num_targets = sum(i in 1..rows, j in 1..cols)(grid[i,j] = TARGET);

% Constraint: All targets must be hit
constraint forall(i in 1..rows, j in 1..cols where grid[i,j] = TARGET) (
    target_hit[i,j] = 1
);

% Constraint: Power sources generate beams
constraint forall(i in 1..rows, j in 1..cols where grid[i,j] = SOURCE) (
    beam_path[i,j] > 0
);

% Constraint: Empty cells can have beams pass through
constraint forall(i in 1..rows, j in 1..cols where grid[i,j] = EMPTY) (
    beam_path[i,j] >= 0
);

% Constraint: Redirectors change beam direction
constraint forall(i in 1..rows, j in 1..cols where grid[i,j] = REDIRECTOR) (
    % If beam enters redirector, it must exit in allowed direction
    (beam_path[i,j] > 0) -> (redirector_angle[i,j] >= 0 /\ redirector_angle[i,j] <= 3)
);

% Constraint: Targets are hit if beam passes through them
constraint forall(i in 1..rows, j in 1..cols where grid[i,j] = TARGET) (
    target_hit[i,j] = (beam_path[i,j] > 0)
);

% Constraint: Beam continuity (simplified)
% For each cell with a beam, check if it connects properly to adjacent cells
constraint forall(i in 1..rows, j in 1..cols where beam_path[i,j] > 0) (
    % Horizontal beam
    (beam_path[i,j] = 1) -> (
        (j > 1 -> beam_path[i,j-1] in {0,1,3,4}) /\
        (j < cols -> beam_path[i,j+1] in {0,1,3,4})
    ) /\
    % Vertical beam
    (beam_path[i,j] = 2) -> (
        (i > 1 -> beam_path[i-1,j] in {0,2,3,4}) /\
        (i < rows -> beam_path[i+1,j] in {0,2,3,4})
    )
);

% Objective: Minimize number of active redirectors
var int: active_redirectors = sum(i in 1..rows, j in 1..cols where grid[i,j] = REDIRECTOR)(
    beam_path[i,j] > 0
);

solve minimize active_redirectors;

% Output
output [
    "Grid layout:\n"
] ++
[
    if j = 1 then "\n" else " " endif ++
    if grid[i,j] = SOURCE then "S"
    elseif grid[i,j] = TARGET then "T"
    elseif grid[i,j] = REDIRECTOR then "R"
    else "."
    endif
    | i in 1..rows, j in 1..cols
] ++
[
    "\n\nSolution found!\n",
    "Active redirectors: " ++ show(active_redirectors) ++ "\n",
    "Targets hit: " ++ show(sum(i in 1..rows, j in 1..cols)(target_hit[i,j])) ++ "/" ++ show(num_targets) ++ "\n"
];
