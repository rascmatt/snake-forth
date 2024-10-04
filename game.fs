: print-full ( -- )
  226 emit 150 emit 136 emit 
  226 emit 150 emit 136 emit ;

: print-empty ( -- )
  32 emit 32 emit ;

: print-board { n1 n2 n -- }
  \ print a square board of size n with the cell at index (n1, n2) highlighted
  n 0 u+do
    10 emit \ line break
    i n 0 u+do
      i n1 = if 
        dup n2 = if
            print-empty
        else 
            print-full
        then
      else
        print-full
      then
    loop
    drop \ drop the outer index
  loop ;

: get-char ( -- )
  ." Press a key: "  \ Prompt the user
  KEY                \ Read a key
;

: clear-screen ( -- )
  27 EMIT  \ Emit the ESC character (ASCII code 27)
  ." [2J"  \ Send the "[2J" sequence to clear the screen
;

: sleep ( n -- )  \ n is the number of milliseconds to sleep
  ms
;

: inc ( addr n -- )
  \ increase the value at specified address with max value n
  over @ <= if
    drop exit \ do nothing
  then
  dup @ 1+ over ! drop ;

: dec ( addr n -- )
  \ decrease the value at specified address with min value n
  over @ >= if
    drop exit \ do nothing
  then
  dup @ 1- over ! drop ;

: handle-direction-keys { a1 a2 n c -- }
  \ change the increase the values att a1 and a2 if c is a direction key (W,A,S,D). n is the board size
  c 'a' = IF
      a1 0 dec
  ELSE 
      c 'd' = IF
          a1 n 1- inc
      ELSE
          c 'w' = IF 
              a2 0 dec
          ELSE
              c 's' = IF 
                  a2 n 1- inc
              THEN        
          THEN    
      THEN
  THEN ;

\ Create global variables
create x 0 ,  \ x position of current index
create y 0 ,  \ y position of current index

: game-loop { n -- }
  \ Start the game loop

  \ Reset index
  0 x !
  0 y !

  BEGIN
    
    \ Check for user input
    KEY? IF
      KEY
      DUP 'q' = IF
        DROP EXIT  \ Exit the loop if 'q' is pressed
      ELSE
        >r x y n r>
        handle-direction-keys
      THEN
    THEN
    
    \ Game Loop
    
    clear-screen
    x @ y @ n print-board

    80 ms  \ Sleep for 80 milliseconds between frames
  AGAIN
;
