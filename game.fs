: print-full ( -- )
  226 emit 150 emit 136 emit 
  226 emit 150 emit 136 emit ;

: print-empty ( -- )
  32 emit 32 emit ;

: print-light-shade ( -- )
  226 emit 150 emit 145 emit
  226 emit 150 emit 145 emit ;

: print-board { board n -- }
  \ print a square board of size n where every non-zero value is highlighed
  n 0 u+do
    10 emit \ line break
    i n 0 u+do
      board over n * cells + i cells + @
      dup 0= if
        drop
        print-full
      else
        -2 = if
          print-light-shade
        else
          print-empty
        then
      then
    loop
    drop \ drop the outer index
  loop ;

: clear-screen ( -- )
  27 EMIT  \ Emit the ESC character (ASCII code 27)
  ." [2J"  \ Send the "[2J" sequence to clear the screen
;

: get-direction { d1 c -- d2 }
  \ resolve direction d2 from key press (0, 1, 2, 3, 4), otherwise default to the previous direction d1
  \ if the direction is the opposite as the current direction, ignore it
  c 'a' = IF
    d1 2 <> IF 1 ELSE d1 THEN
  ELSE 
    c 'd' = IF
      d1 1 <> IF 2 ELSE d1 THEN
    ELSE
      c 'w' = IF 
        d1 4 <> IF 3 ELSE d1 THEN
      ELSE
        c 's' = IF 
          d1 3 <> IF 4 ELSE d1 THEN
        ELSE
          d1
        THEN
      THEN    
    THEN
  THEN ;

: next-location { a1 a2 d n -- a1 a2 }
  \ Calcualte the next location for 'a2' based on the given direction 'd' and grid at 'a1' with size 'n'

  a2 a1 - cell /  \ calc the grid offset
  n /mod          \ split into x and y coordinates
  { x y }
  
  a2

  d 1 = IF              \ Move left
    x 0= IF
      dup n 1- cells +  \ Wrap around
    ELSE 
      dup cell -        \ Move left
    THEN
    exit
  THEN

  d 2 = IF              \ Move right
    x n 1- = IF
      dup n 1- cells -  \ Wrap around
    ELSE 
      dup cell +        \ Move right
    THEN
    exit
  THEN

  d 3 = IF                  \ Move up
    y 0= IF
      dup n n 1- * cells +  \ Wrap around
    ELSE 
      dup n cells -         \ Move up
    THEN
    exit
  THEN
  
  y n 1- = IF
    dup n n 1- * cells -  \ Wrap around
  ELSE
    dup n cells +         \ Move down
  THEN
;

: move-snake { head a1 a2 b -- }
  \ Move the snake with head at a1 to the new location a2.
  \ Decide wheter to grow the snake (i.e. the tail will not be moved) based on the flag 'b'

  \ Do nothing if the new location is the same as the old one
  a1 a2 = IF
    EXIT
  THEN

  \ update the head pointer to point to the new address
  a2 head !
  \ update the new head to point to the old head
  a1 a2 !

  b 0<> IF
    EXIT
  THEN
  
  ( )
  a1
  ( a1 )
  dup @ ( a1 a2 )
  begin
    dup @ -1 <> while
    nip ( a2 )
    dup @ ( a2 a3 )
  repeat

  ( a1 a2 )
  0 swap ! ( a1 )
  true swap !
  ( )
;

: spawn-fruit { board n -- }
  \ Spawn a fruit at some empty location
  
  begin

    \ Pick a random number in [0; n*n]
    utime drop n dup * mod
    { r }
    
    \ Calculate the address
    board r cells +

    \ Only set if the position is empty
    dup @ 0= IF 
      -2 swap !
      exit
    THEN

  again
;

: game-loop { board head n -- }
  \ Run the game loop

  \ Initialize the loop counter
  1

  \ Initialize the direction to move right
  2

  BEGIN

    ( i d )
    
    \ Check for user input
    KEY? IF
      KEY
      DUP 'q' = IF
        DROP EXIT  \ Exit the loop if 'q' is pressed
      ELSE
        ( d c )
        get-direction
        ( d )
      THEN
    THEN

    \ Update the game state

    \ calculate next locaion based on current position & user input
    ( )
    dup board swap
    ( board dir )
    head @ swap n
    ( board a1 dir n )
    next-location
    ( a1 a0 )

    head -rot
    ( head a1 a0 )

    \ check for self collision (i.e. not empty and not a fruit)
    dup @ dup 0<> swap -2 <> and 
    IF
      10 emit
      ." Game Over"
      EXIT
    THEN
    
    \ the snake should grow if the next location contains a fruit
    dup @ -2 =
    { grow }
    
    \ move the snake to the new location
    ( head a1 a0 )
    grow move-snake

    \ span the next fruit
    grow IF 
      board n spawn-fruit
    THEN
    
    \ Render the game state
    
    clear-screen
    board n print-board

    \ Increase the loop counter
    swap 1+ swap

    \ Limit fps
    400 ms
  AGAIN
;

: start { n -- }
  \ Start a game with a board size of n
  
  \ allocate memory for the board
  here n dup * cells allot

  \ clear board
  n 0 u+do
    i n 0 u+do
      over over n * cells + i cells + 0 swap !
    loop drop
  loop

  \ initialize the snake (the value -1 signals the tail of the snake)
  dup -1 swap !     \ first cell is the tail
  dup dup cell + !  \ second cell is the head and points at tail

  \ initialize the fruit
  dup n spawn-fruit

  \ store address of the head
  here cell allot
  
  dup third cell + swap !
  
  n game-loop ;
