using System;
using System.Buffers;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

class Program{
    public static async Task Main(string[] args){
        ContactManager manager = new ContactManager();
        bool running = true;

        Console.WriteLine("Welcome to the Contact Manager!");
        Console.WriteLine("To see the list of commands, type 'help'");

        while (running)
        {
            Console.Write("Enter command: ");
            string input = Console.ReadLine();
            string[] parts = input.Split(new char[] { ' ' }, 3);

            if (parts.Length == 0)
            {
                continue;
            }

            string command = parts[0].ToLower();
            try
            {
                switch (command)
                {
                    case "help":
                        Console.WriteLine("Available commands:");
                        Console.WriteLine("Add_Contact [number] [name] - Add a new contact");
                        Console.WriteLine("Remove [number] [name] - Remove a contact");
                        Console.WriteLine("Search_by_Name [name] - Search for a contact by name");
                        Console.WriteLine("Search_by_Number [number] - Search for a contact by number");
                        Console.WriteLine("Update_Contact_Name [number] [new name] - Update a contact's name");
                        Console.WriteLine("Print_All - Print all contacts");
                        Console.WriteLine("Exit - Exit the program");
                        break;
                    case "add_contact":
                        if (parts.Length < 3) throw new FormatException("Add command format: Add [number] [name]");
                        long addNumber = long.Parse(parts[1]);
                        string addName = parts[2];
                        bool added = manager.AddContact(addNumber, addName);
                        Console.WriteLine(added ? "Contact added successfully." : "Contact already exists.");
                        break;

                    case "remove":
                        if (parts.Length < 3) throw new FormatException("Remove command format: Remove [number] [name]");
                        long removeNumber = long.Parse(parts[1]);
                        string removeName = parts[2];
                        bool removed = manager.RemoveContact(removeNumber, removeName);
                        Console.WriteLine(removed ? "Contact removed successfully." : "Contact not found.");
                        break;

                    case "search_by_name":
                        if (parts.Length < 2) throw new FormatException("Search by Name command format: Search by Name [name]");
                        string searchName = parts[1];
                        string resultName = manager.SearchContactByName(searchName);
                        Console.WriteLine(resultName);
                        break;

                    case "search_by_number":
                        if (parts.Length < 2) throw new FormatException("Search by Number command format: Search by Number [number]");
                        long searchNumber = long.Parse(parts[1]);
                        string resultNumber = manager.SearchContactByNumber(searchNumber);
                        Console.WriteLine(resultNumber);
                        break;

                    case "update_contact_name":
                        if (parts.Length < 3) throw new FormatException("Update command format: Update [number] [new name]");
                        long number = long.Parse(parts[1]);
                        string NewName = parts[2];
                        bool updated = manager.UpdateContactName(number, NewName);
                        Console.WriteLine(updated ? "Contact updated successfully." : "Contact not found.");
                        break;

                    case "print_all":
                        manager.PrintAllContacts();
                        break;

                    case "exit":
                        running = false;
                        break;

                    default:
                        Console.WriteLine("Unknown command. Please try again.");
                        break;
                }
            }
            catch (FormatException fe)
            {
                Console.WriteLine(fe.Message);
            }
            catch (ArgumentException ae)
            {
                Console.WriteLine(ae.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }

        Console.WriteLine("Thank you for using Contact Manager!");
    }

    public class ContactManager
    {
        private MyHashSet contactSet;

        public ContactManager()
        {
            contactSet = new MyHashSet();
        }

        public bool AddContact(long number, string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Name cannot be null or empty", nameof(name));
            }

            if (number <= 0 || number.ToString().Length != 9)
            {
                throw new ArgumentException("Number must be a positive 9-digit number", nameof(number));
            }

            return contactSet.Add(number, name);
        }

        public bool RemoveContact(long number, string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Name cannot be null or empty", nameof(name));
            }

            if (number <= 0 || number.ToString().Length != 9)
            {
                throw new ArgumentException("Number must be a positive 9-digit number", nameof(number));
            }

            return contactSet.Remove(number, name);
        }

        public void PrintAllContacts()
        {
            if (contactSet.Count == 0)
            {
                throw new InvalidOperationException("No contacts to print");
            }
            contactSet.PrintAll();
        }

        public string SearchContactByName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Name cannot be null or empty", nameof(name));
            }            
            var result = contactSet.SearchItemByName(name);
            if (result.HasValue)
            {
                return $"Number: {result.Value.number}, Name: {result.Value.name}";
            }
            else
            {
                return "Contact not found.";
            }
        }
        public string SearchContactByNumber(long number)
        {
            if (number <= 0 || number.ToString().Length != 9)
            {
                throw new ArgumentException("Number must be a positive 9-digit number", nameof(number));
            }
            var result = contactSet.SearchItemByNumber(number);
            if (result.HasValue)
            {
                return $"Number: {result.Value.number}, Name: {result.Value.name}";
            }
            else
            {
                return "Contact not found.";
            }
        }

        public bool UpdateContactName(long number, string newName)
        {
            if (string.IsNullOrEmpty(newName))
            {
                throw new ArgumentException("Name cannot be null or empty", nameof(newName));
            }

            if (number <= 0 || number.ToString().Length != 9)
            {
                throw new ArgumentException("Number must be a positive 9-digit number", nameof(number));
            }

            return contactSet.UpdateItemName(number, newName);
        }

    }
    public class MyHashSet
    {
        private const int _initialCapacity = 10;
        private LinkedList<(Int64 number, string name)>[] buckets;
        private int _count;
        private int _capacity;
    
        public int Count => _count;
    
        public MyHashSet()
        {
            _capacity = _initialCapacity;
            buckets = new LinkedList<(Int64 number, string name)>[_capacity];
            _count = 0;
        }
    
        private int GetBucketIndex(Int64 item)
        {
            int hashCode = GetHashCode(item);
            return hashCode % buckets.Length;
        }
    
        private int GetHashCode(Int64 item)
        {
            return item.GetHashCode();
        }

        public void PrintAll()
        {
            foreach (var bucket in buckets)
            {
                if (bucket != null)
                {
                    foreach (var item in bucket)
                    {
                        Console.WriteLine($"Number: {item.number}, Name: {item.name}");
                    }
                }
            }
        }

        public (Int64 number, string name)? SearchItemByName(string name)
        {
            foreach (var bucket in buckets)
            {
                if (bucket != null)
                {
                    foreach (var item in bucket)
                    {
                        if (item.name == name)
                        {
                            return item;
                        }
                    }
                }
            }
            return null;
        }

        public (Int64 number, string name)? SearchItemByNumber(Int64 number)
        {
            int bucketIndex = GetBucketIndex(number);
            if (buckets[bucketIndex] != null)
            {
                foreach (var item in buckets[bucketIndex])
                {
                    if (item.number == number)
                    {
                        return item;
                    }
                }
            }
            return null;
        }

        public bool UpdateItemName(Int64 number, string newName)
        {
            var item=SearchItemByNumber(number);
            if (item.HasValue)
            {
                Remove(number, item.Value.name);
                Add(number, newName);
                return true;
            }
            return false;
        }
        public bool Add(Int64 item, string name)
        {
            if (Contains(item, name))
            {
                return false;
            }
    
            if (_count >= _capacity)
            {
                IncreaseCapacity();
            }
    
            int bucketIndex = GetBucketIndex(item);
    
            if (buckets[bucketIndex] == null)
            {
                buckets[bucketIndex] = new LinkedList<(Int64 number, string name)>();
            }
    
            buckets[bucketIndex].AddLast((item, name));
            _count++;
            return true;
        }
    
        public bool Remove(Int64 item, string name)
        {
            int bucketIndex = GetBucketIndex(item);
    
            if (buckets[bucketIndex] != null)
            {
                var bucket = buckets[bucketIndex];
                return bucket.Remove((item, name));
            }
    
            return false;
        }
    
        public bool Contains(Int64 item, string name)
        {
            int bucketIndex = GetBucketIndex(item);
    
            if (buckets[bucketIndex] != null)
            {
                return buckets[bucketIndex].Contains((item, name));
            }
    
            return false;
        }
    
        private void IncreaseCapacity()
        {
            _capacity = buckets.Length * 2;
            LinkedList<(Int64 number, string name)>[] newBuckets = new LinkedList<(Int64 number, string name)>[_capacity];
    
            foreach (var bucket in buckets)
            {
                if (bucket != null)
                {
                    foreach (var item in bucket)
                    {
                        int newBucketIndex = GetBucketIndex(item.number);
    
                        if (newBuckets[newBucketIndex] == null)
                        {
                            newBuckets[newBucketIndex] = new LinkedList<(Int64 number, string name)>();
                        }
                        newBuckets[newBucketIndex].AddLast(item);
                    }
                }
            }
            buckets = newBuckets;
        }
    }
    
}
