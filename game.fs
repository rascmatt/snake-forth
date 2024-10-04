: print-full ( -- )
  226 emit 150 emit 136 emit 
  226 emit 150 emit 136 emit ;

: print-empty ( -- )
  32 emit 32 emit ;

: print-checkered-board { n -- }
  \ print a checkered square board of size n
  n 0 u+do
    10 emit \ line break
    i n 0 u+do
      i 2 mod 0= if 
        dup 2 mod 0= if
            print-full
        else 
            print-empty
        then
      else
        dup 2 mod 0= if
            print-empty
        else 
            print-full
        then
      then
    loop
    drop \ drop the outer index
  loop ;

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


\ Create global index for current position
create x 0 ,
create y 0 ,

: game-loop { n -- }
  \ Start the game loop with a board size of n

  \ Reset index
  0 x !
  0 y !

  BEGIN
    
    \ Check for user input
    \ TODO: extract to separate word
    KEY? IF
      KEY
      DUP 'q' = IF
        DROP EXIT  \ Exit the loop if 'q' is pressed
      ELSE
        DUP 'a' = IF
            x 0 dec
        ELSE 
            DUP 'd' = IF 
                x n 1- inc
            ELSE
                DUP 'w' = IF 
                    y 0 dec
                ELSE
                    DUP 's' = IF 
                        y n 1- inc
                    THEN        
                THEN    
            THEN
        THEN
      THEN
      DROP
    THEN
    
    \ Game Loop
    
    clear-screen
    x @ y @ n print-board

    100 ms  \ Sleep for 100 milliseconds between frames
  AGAIN
;
