# Example 1: Adding Contacts

Add_Contact 123456789 John
>Contact added successfully.

Add_Contact 987654321 Alice
>Contact added successfully.

Add_Contact 123456789 John
>Contact already exists.

Add_Contact 000000000 Invalid
>Error: Number must be a positive 9-digit number

# Example 2: Removing Contacts

Add_Contact 111222333 Mary
>Contact added successfully.

Remove 111222333 Mary
>Contact removed successfully.

Remove 111222333 Mary
>Contact not found.

RemoveContact 000000000 Invalid
>Error: Number must be a positive 9-digit number

# Example 3: Searching for Contacts

Add_Contact 222333444 Jane
>Contact added successfully.

Search_By_Name Jane
>Number: 222333444, Name: Jane

Search_By_Name Bob
>Contact not found.

Search_By_Number 222333444
>Number: 222333444, Name: Jane

Search_By_Number 555666777
>Contact not found.

# Example 4: Updating Contact Names

Add_Contact 333444555 David
>Contact added successfully.

Update 333444555 Dave
>Contact updated successfully.

Search_By_Number 333444555
>Number: 333444555, Name: Dave

Update 111111111 NewName
>Contact not found.

# Example 5: Listing All Contacts

Add_Contact 444555666 Eve
>Contact added successfully.

Print_All
>Number: 123456789, Name: John
Number: 987654321, Name: Alice
Number: 222333444, Name: Jane
Number: 333444555, Name: Dave
Number: 444555666, Name: Eve
