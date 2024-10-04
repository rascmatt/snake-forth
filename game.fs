: print-full ( -- )
  226 emit 150 emit 136 emit 
  226 emit 150 emit 136 emit ;

: print-empty ( -- )
  32 emit 32 emit ;

: print-board { board n -- }
  \ print a square board of size n where every non-zero value is highlighed
  n 0 u+do
    10 emit \ line break
    i n 0 u+do
      board over n * cells + i cells + @
      0= if
        print-full
      else
        print-empty
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
  c 'a' = IF
    1
  ELSE 
    c 'd' = IF
      2
    ELSE
      c 'w' = IF 
        3
      ELSE
        c 's' = IF 
          4
        ELSE
          d1
        THEN
      THEN    
    THEN
  THEN ;

: next-location { a d n -- a1 a2 }
  \ Calcualte the next location for 'a' based on the given direction 'd' and grid size 'n'
  
  a

  d 1 = IF
    dup cell -     \ Move left
    exit
  THEN
  
  d 2 = IF
    dup cell +     \ Move right
    exit
  THEN

  d 3 = IF
    dup n cells -  \ Move down
    exit
  THEN
  
  dup n cells +    \ Move up
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

    dup 0 <> IF
      
      \ calculate next locaion based on current position & user input
      ( )
      dup head @ swap n
      ( a1 dir n )
      next-location
      ( a1 a0 )

      head -rot
      ( head a1 a0 )
      
      \ decide if the snake should grow (it grows every 5 iterations)
      ( [i d] head a1 a0 )
      >r fourth r> swap     \ copy the loop counter to the top, it's at the 5'th position
      5 mod 0=
      
      \ move the snake to the new location
      ( head a1 a0 b )
      move-snake
      
    THEN
    
    \ Render the game state
    
    clear-screen
    board n print-board

    \ Increase the loop counter
    swap 1+ swap

    \ Limit fps
    200 ms
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

  \ store address of the head
  here cell allot
  
  dup third cell + swap !
  
  n game-loop ;
