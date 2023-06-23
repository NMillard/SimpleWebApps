# Business rules in software development

Business rules are central to most applications. One thing that all business rules have in common is they change. Often
and sometimes with profound effects on existing data.

This solution demonstrates a few simple examples of how to implement business rules and policies with varying degrees of
flexibility.

We'll look at a few examples concerned with "usernames" since I supposed most are quite familiar with the concept of
creating and logging in with a username in various online services.

### The business rules

We start out with having just a few, very simple rules, a username:

1. cannot be no value (null) or an empty string.
2. may not exceed 50 characters.
3. may at minimum be 3 characters.

### Additional requirements pop up

A new requirement pops up. The business wants a small additional addition.

Usernames must now:

5. cannot contain special characters. Only alphanumeric characters are allowed.

Some time goes by, say, a few years. The business is thriving and usernames have sort of become a commodity, that may
generate a few extra bucks to the business.  

So, the business decides that it wants to offer premium usernames, that can contain special characters, and by the way,
the business now also wants to make minimum and maximum lengths configurable - without having to recompile or restart
the application.

Actually, the business wants to have the option to switch the business rules off and on as they please.

## Building flexibility

Every business rule and policy should be its own class, that is easily debugged, swapped, replaced, turned on and off.