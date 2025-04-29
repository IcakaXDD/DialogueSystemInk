INCLUDE global.ink


~playEmote("exclamation")

Hello there! #speaker: Dr.Green #portrait:Dr_green_neutral #layout:left
->main

===main===
How are you feeling today?
+[Happy]
    ~playEmote("exclamation")
    That makes me fell <color=\#F8FF30>happy</color> as well! #portrait:Dr_green_smile
+[Sad] 
    Oh, well that makes me <color=\#%B81FF>sad</color> too. #portrait:Dr_green_sad
- Don't trust him, he is <color=\#FF1E35>not</color> a real doctor! #speaker:Ms. Yellow #portrait:MsYellow_neutral #layout:right
~playEmote("exclamation")
Well, do you have any more questions? #speaker:Dr. Green #portrait:Dr_green_neutral #layout:left
+[Yes]
    ->main
+[No]
    Goodbye then!
    ->END