# Track

An app to organize track events.

## Notes to myself - TODO

Site colors - http://colormind.io/bootstrap/ see that site for instructions on
how to use it. `EDE8EA` `C4ADAD` `EEEEEE` `988983` `527597`

Logo maker - logomakr.com/5U2Emq - https://my.logomakr.com/give_credit

Minimalist CSS frameworks:


**<https://picnicss.com/documentation>**  
https://minicss.org/docs  
http://beauter.outboxcraft.com/  
https://cmroanirgo.github.io/inviscss/  
https://vitalcss.com/

**Icons**

https://iconsvg.xyz/  
https://loading.io/

## Front end behavior

Command and control from the back end as much as possible

- Be able to toggle form edit/presentation without interaction from back end
    + Make sure submit button is disabled
- Be able to command from back end
    + Headers:
        * X-Target: '#MyTarget' -- could be element and it matches all elements
          with same value
        * X-Action: Append, Replace, ReplaceIn, Prepend
        * X-Reset-From
        * X-Disable-Submit
        * ~~X-Enable-Submit~~ // Redundant, not needed
- Be able to toggle class when form submit showing that it is submitting
- Show if successful or failure messages


