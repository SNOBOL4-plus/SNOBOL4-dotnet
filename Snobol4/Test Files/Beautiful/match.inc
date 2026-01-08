*//-----------------------------------------------------------------------------------------------------------------------
*// $Workfile: match.inc $ $Revision: 3 $ $Date: 3/13/05 5:32p $ 
*//-----------------------------------------------------------------------------------------------------------------------
*//
*// Pattern matching during pattern matching:
*// pattern $ variable $ *match(variable, pattern)
*// pattern $ variable $ *notmatch(variable, pattern)
*//
*//----------- --------------------------------------------------------------------- -----------------
               DEFINE('match(subject,pattern)')                                      :(match_end)
match          match          =  .dummy
               subject        pattern                                                :S(NRETURN)F(FRETURN)
match_end
*//----------- --------------------------------------------------------------------- -----------------
               DEFINE('notmatch(subject,pattern)')                                   :(notmatch_end)
notmatch       notmatch       =  .dummy
               subject        pattern                                                :S(FRETURN)F(NRETURN)
notmatch_end
