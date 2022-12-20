# Why did you do X in the 4.0.1 release?

## TLDR;
- Found a bug.
- Wrote a fix that requires an Environment Variable named PIPELINE_PreventCollisionsWithRenamedValueNames to turn on for 4.0.x.
- Plan to require an Environment Variable named PIPELINE_AllowCollisionsWithRenamedValueNames to turn the fix OFF in 5.x, whenever that is released.


## Why, oh why did you do something?
- Why did you add the PIPELINE_PreventCollisionsWithRenamedValueNames with a default value of false to RenameContextItemStep.cs
- Why didn't you add this configuration value to the appropriate ~Options class?
- What is the purpose of the PreventCollisionsWithRenamedValueNames changes in the first place?

When testing an application, I used the MoveOutputObjectToInputObjectStep multiple times within the same pipeline.
I noticed an exception being thrown about "PIPELINE_INPUT_OBJECT already exists." on the application.
As I reviewed the RenameContextItemStep, I noticed that the Key names were being cached, but not the Value names.
If an earlier step created the PIPELINE_INPUT_OBJECT key and I ran a Rename/Move step then we had a name collision and KA-BOOM!

The current behavior has side-effects that developers may be using presently.

## CATCH-22! Which left me with three options for fixing the behavior.

1. I can create a breaking change (5.0.0) and deprecate this behavior, but upgrade everything to support it.
2. I can create a breaking change (5.0.0) and continue to support this behavior, but make it a "use at your own risk" and "you have to turn it on" behavior.
3. I can create a non-breaking change in 4.0.x and allow developers to reconfigure themselves to a safer model.  If they aren't experiencing problems, no work is necessary.

I chose option #3 at the present time and deferred the future decision with bias towards #2.

## INTERIM SOLUTION - or so the thinking goes...
The first solution was to add a property to RenameContextItemOptions called PreventCollisionsWithRenamedValueNames which could be used with RenameContextItemStep
and have the added benefit of being configurable at the Step level.  This idea quickly led into a collision of handling it one way for RenameContextItemStep and
a different way for all of the MoveInput~ToOutput~Steps.  

## Why did the INTERIM SOLUTION fail?
As the fix was developed, the earlier decision to hard-code the RenameContextItemOptions rather than injecting has proved short-sighted.
While hard-coding requires no other configuration at run-time, any NEW values added to RenameContextItemOptions are unavailable for override via any available 
IConfiguration model without a change to the constructor.

## BACK TO GUIDING PRINCIPLES!
My guiding principle for this project is that there may be multiple ways to do many things, but the easiest way should handle 80% of the cases.
A little bit of effort (and thinking) should be required to handle the next 10% of cases.
And the final 10% of cases are super-flexible, but cause rabbit-holes, difficulties with the other 90% of cases, and aren't worth my effort to implement.

The solution became clear and relatively simple.

## SOLUTION FOR 4.x
Use an environment variable with a specified name 'PIPELINE_PreventCollisionsWithRenamedValueNames' to turn on the collision-prevention behavior for 4.x 
for every instance of any step derived from RenameContextItemStep.  The behavior remains consistent.  

The lack of this environment variable with the value of 'True' means the code works EXACTLY like it did prior to the 4.0.x release.

## PROPOSED SOLUTION FOR 5.x and up.
Use an environment variable with a specified name 'PIPELINE_AllowCollisionsWithRenamedValueNames' to turn OFF the collision-prevention behavior for 5.x 
for every instance of any step derived from RenameContextItemStep.

The lack of this environment variable with the value of 'True' means that collision prevention behavior works like 4.0.x WITH the 'Prevent' Environment variable configured.







