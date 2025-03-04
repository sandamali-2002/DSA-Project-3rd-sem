using System;
using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace HotelManagementSystem
{
    public class Hotel
    {
        // Nested Node class that represents a room record.
        private class Node
        {
            public int Id { get; set; }
            public DateTime Date { get; set; }
            public string? Name { get; set; }
            public string? RoomType { get; set; }
            public bool IsAvailable { get; set; }
            public Node? Next { get; set; }
        }

        // Head pointer for the linked list.
        private Node? head = null;

        public void Menu()
        {
            while (true)
            {
                Stopwatch stopwatch = Stopwatch.StartNew(); // Start timing


                Console.WriteLine();
                Console.WriteLine("\tWelcome to Hotel Management System Application");
                Console.WriteLine();
                Console.WriteLine("\t____Hotel Management System____");
                Console.WriteLine();
                Console.WriteLine("S.No\tFunctions\t\t\tDescription");
                Console.WriteLine("1\tAllocate Room\t\t\tInsert New Room");
                Console.WriteLine("2\tSearch Room\t\t\tSearch Room with Room ID");
                Console.WriteLine("3\tBooking Update\t\t\tUpdate a Booking Record");
                Console.WriteLine("4\tBooking Delete\t\t\tDelete a Booking Record");
                Console.WriteLine("5\tShow Room Records\t\tShow Room Records (Sorted by Date)");
                Console.WriteLine("6\tBook Room\t\t\tBook an Available Room");
                Console.WriteLine("7\tShow Available Rooms\t\tShow Available Rooms");
                Console.WriteLine("8\tSave to CSV\t\t\tSave Data to CSV File");
                Console.WriteLine("9\tLoad from CSV\t\t\tLoad Data from CSV File");
                Console.WriteLine("10\tExit");
                Console.Write("Enter Your Choice: ");

                if (!int.TryParse(Console.ReadLine(), out int choice))
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        Insert();
                        break;
                    case 2:
                        Search();
                        break;
                    case 3:
                        Update();
                        break;
                    case 4:
                        Delete();
                        break;
                    case 5:
                        MeasureExecutionTime(() => { SortByDate(); Show(); });

                        Show();
                        break;
                    case 6:
                        BookRoom();
                        break;
                    case 7:
                        ShowAvailableRooms();
                        break;
                    case 8:
                        SaveToCSV();
                        break;
                    case 9:
                        LoadFromCSV();
                        break;
                    case 10:
                        Console.WriteLine("Exiting and saving data...");
                        SaveToCSV(); // Save to CSV by default before exiting
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Invalid choice");
                        break;
                }

                stopwatch.Stop(); // Stop timing
                Console.WriteLine($"Menu Execution Time: {stopwatch.ElapsedMilliseconds} ms");


            }
        }


        private void MeasureExecutionTime(Action action)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            action();
            stopwatch.Stop();
            Console.WriteLine($"Execution Time: {stopwatch.ElapsedMilliseconds} ms\n");
        }




        // Inserts a new room record into the linked list with unique ID check.
        public void Insert()
        {
            Console.WriteLine("\n\t____Hotel Management System____");
            Node newNode = new Node();

            Console.Write("Enter Room ID: ");
            int id;
            while (!int.TryParse(Console.ReadLine(), out id))
            {
                Console.Write("Invalid input. Enter a valid Room ID: ");
            }
            newNode.Id = id;

            Node? ptr = head;
            while (ptr != null)
            {
                if (ptr.Id == newNode.Id)
                {
                    Console.WriteLine("Room ID already exists! Please use a different ID.");
                    return;
                }
                ptr = ptr.Next;
            }

            Console.Write("Enter Room type (Single/Double/Twin): ");
            newNode.RoomType = Console.ReadLine();

            newNode.IsAvailable = true; // New room is available
            newNode.Name = null; // No customer yet
            newNode.Date = DateTime.MinValue; // No booking date yet
            newNode.Next = null;

            if (head == null)
            {
                head = newNode;
            }
            else
            {
                ptr = head;
                while (ptr.Next != null)
                {
                    ptr = ptr.Next;
                }
                ptr.Next = newNode;
            }

            Console.WriteLine("\nNew Room Inserted!\n");
        }

        // New BookRoom function
        public void BookRoom()
        {
            Console.WriteLine("\n\t__Hotel Room Booking__");
            if (head == null)
            {
                Console.WriteLine("No Rooms Exist. Please add rooms first");
                return;
            }

            ShowAvailableRooms();
            Console.Write("\nEnter Room ID to Book: ");
            if (!int.TryParse(Console.ReadLine(), out int roomId))
            {
                Console.WriteLine("Invalid Room ID.");
                return;
            }

            Node? ptr = head;
            while (ptr != null)
            {
                if (ptr.Id == roomId)
                {
                    if (!ptr.IsAvailable)
                    {
                        Console.WriteLine("Room is Already Booked!");
                        return;
                    }
                    Console.Write("Enter Customer Name: ");
                    ptr.Name = Console.ReadLine();

                    Console.Write("Enter Booking Date (e.g., YYYY-MM-DD):");
                    DateTime date;
                    while (!DateTime.TryParse(Console.ReadLine(), out date))
                    {
                        Console.Write("Invalid Input. Enter a valid date (e.g., YYYY-MM-DD):");
                        return;
                    }

                    ptr.Date = date;
                    ptr.IsAvailable = false;
                    Console.WriteLine($"\nRoom {ptr.Id} booked successfully!");
                    return;
                }
                ptr = ptr.Next;
            }
            Console.WriteLine("Room ID not found.");
        }

        // New ShowAvailableRooms function
        public void ShowAvailableRooms()
        {
            Console.WriteLine("\n\t__Available Rooms__");
            if (head == null)
            {
                Console.WriteLine("No Rooms Exist in the System.");
                return;
            }

            Node? ptr = head;
            bool hasAvailable = false;
            while (ptr != null)
            {
                if (ptr.IsAvailable)
                {
                    Console.WriteLine($"\nRoom ID: {ptr.Id}");
                    Console.WriteLine($"Room Type: {ptr.RoomType}");
                    hasAvailable = true;
                }
                ptr = ptr.Next;
            }

            if (!hasAvailable)
            {
                Console.WriteLine("No Rooms are Currently Available.");
            }
        }

        // Searches for a room record by Room ID.
        public void Search()
        {
            Console.WriteLine("\n\t___Hotel Management System___");
            if (head == null)
            {
                Console.WriteLine("\nLinked list is empty");
                return;
            }

            Console.Write("\nEnter Room ID to search: ");
            if (!int.TryParse(Console.ReadLine(), out int t_id))
            {
                Console.WriteLine("Invalid Room ID.");
                return;
            }

            Node? ptr = head;
            while (ptr != null)
            {
                if (t_id == ptr.Id)
                {
                    Console.WriteLine("\nRoom ID: " + ptr.Id);
                    Console.WriteLine("Customer Name: " + (ptr.Name ?? "Not Booked"));
                    Console.WriteLine("Room Allocated Date: " + (ptr.Date == DateTime.MinValue ? "Not Booked" : ptr.Date.ToString("yyyy-MM-dd")));
                    Console.WriteLine("Room Type: " + ptr.RoomType);
                    Console.WriteLine("Available: " + (ptr.IsAvailable ? "Yes" : "No"));
                    return;
                }
                ptr = ptr.Next;
            }
            Console.WriteLine("Room ID not found.");
        }

        // Updates an existing room record.
        public void Update()
        {
            Console.WriteLine("\n\t___Hotel Management System___");
            if (head == null)
            {
                Console.WriteLine("\nLinked list is empty");
                return;
            }

            Console.Write("\nEnter Room ID to update: ");
            if (!int.TryParse(Console.ReadLine(), out int t_id))
            {
                Console.WriteLine("Invalid Room ID.");
                return;
            }

            Node? ptr = head;
            while (ptr != null)
            {
                if (t_id == ptr.Id)
                {
                    Console.Write("Enter new Room ID: ");
                    int newId;
                    while (!int.TryParse(Console.ReadLine(), out newId))
                    {
                        Console.Write("Invalid Input. Enter a Valid Room ID: ");
                    }

                    // Check if new ID is unique (excluding the current node)
                    Node? checkPtr = head;
                    while (checkPtr != null)
                    {
                        if (checkPtr != ptr && checkPtr.Id == newId)
                        {
                            Console.WriteLine("New Room ID already exists! Update aborted.");
                            return;
                        }
                        checkPtr = checkPtr.Next;
                    }
                    ptr.Id = newId;

                    Console.Write("Enter New Room Type: ");
                    ptr.RoomType = Console.ReadLine();

                    // Only update booking details if room is booked
                    if (!ptr.IsAvailable)
                    {
                        Console.Write("Enter new Customer Name: ");
                        ptr.Name = Console.ReadLine();

                        Console.Write("Enter new Booking Date (e.g., YYYY-MM-DD): ");
                        string? dateInput = Console.ReadLine();
                        if (!DateTime.TryParse(dateInput, out DateTime newDate))
                        {
                            Console.WriteLine("Invalid Input. Enter a Valid Date: ");
                            return;
                        }
                        ptr.Date = newDate;
                    }

                    Console.WriteLine("\nUpdate Record Successfully");
                    return;
                }
                ptr = ptr.Next;
            }
            Console.WriteLine("Room ID not found for Update.");
        }

        // Deletes a room record by Room ID.
        public void Delete()
        {
            Console.WriteLine("\n\t___Hotel Management System___");
            if (head == null)
            {
                Console.WriteLine("\nLinked list is empty");
                return;
            }

            Console.Write("\nEnter Room ID to delete: ");
            if (!int.TryParse(Console.ReadLine(), out int t_id))
            {
                Console.WriteLine("Invalid Room ID.");
                return;
            }

            if (head.Id == t_id)
            {
                head = head.Next;
                Console.WriteLine("Delete Room Record Successful");
                return;
            }
            else
            {
                Node? previous = head;
                Node? current = head.Next;
                while (current != null)
                {
                    if (current.Id == t_id)
                    {
                        previous.Next = current.Next;
                        Console.WriteLine("Delete Room Record Successful");
                        return;
                    }
                    previous = current;
                    current = current.Next;
                }
                Console.WriteLine("Room ID not found for deletion.");
            }
        }
        
        // Sorts the linked list records by Date using a bubble sort approach.
       /* public void SortByDate()
        {
            if (head == null || head.Next == null)
            {
                Console.WriteLine("\nLinked list is empty");
                return;
            }

            int count = 0;
            Node? ptr = head;
            while (ptr != null)
            {
                count++;
                ptr = ptr.Next;
            }

            for (int i = 0; i < count - 1; i++)
            {
                Node? current = head;
                while (current != null && current.Next != null)
                {
                    if (current.Date > current.Next.Date)
                    {
                        int tempId = current.Id;
                        DateTime tempDate = current.Date;
                        string? tempName = current.Name;
                        string? tempRoomType = current.RoomType;
                        bool tempAvailable = current.IsAvailable;

                        current.Id = current.Next.Id;
                        current.Date = current.Next.Date;
                        current.Name = current.Next.Name;
                        current.RoomType = current.Next.RoomType;
                        current.IsAvailable = current.Next.IsAvailable;

                        current.Next.Id = tempId;
                        current.Next.Date = tempDate;
                        current.Next.Name = tempName;
                        current.Next.RoomType = tempRoomType;
                        current.Next.IsAvailable = tempAvailable;
                    }
                    current = current.Next;
                }
            }
        }
       */


        // Sorts the linked list records by Date using a merge sort approach.
        /*
        public void SortByDate()
        {
            head = MergeSort(head);
        }

        private Node? MergeSort(Node? head)
        {
            if (head == null || head.Next == null)
            {
                return head;
            }

            // Split the list into two halves
            Node? middle = GetMiddle(head);
            Node? nextOfMiddle = middle.Next;
            middle.Next = null; // Split into two lists

            // Recursively sort both halves
            Node? left = MergeSort(head);
            Node? right = MergeSort(nextOfMiddle);

            // Merge the sorted halves
            return Merge(left, right);
        }

        private Node? GetMiddle(Node? head)
        {
            if (head == null)
                return head;

            Node? slow = head;
            Node? fast = head.Next;

            while (fast != null && fast.Next != null)
            {
                slow = slow.Next;
                fast = fast.Next.Next;
            }

            return slow;
        }

        private Node? Merge(Node? left, Node? right)
        {
            if (left == null) return right;
            if (right == null) return left;

            Node? result;

            if (left.Date <= right.Date)
            {
                result = left;
                result.Next = Merge(left.Next, right);
            }
            else
            {
                result = right;
                result.Next = Merge(left, right.Next);
            }

            return result;
        }
        */
       
        

        //Sorts the linked list records by Date using a quick sort approach

        
       
        public void SortByDate()
            {
                head = QuickSort(head, GetTail(head));
            }

            // Function to get the last node (tail)
            private Node GetTail(Node node)
            {
                while (node != null && node.Next != null)
                    node = node.Next;
                return node;
            }

            // QuickSort function
            private Node QuickSort(Node start, Node end)
            {
                if (start == null || start == end)
                    return start;

                Node newHead = null, newTail = null;

                Node pivot = Partition(start, end, ref newHead, ref newTail);

                if (newHead != pivot)
                {
                    Node temp = newHead;
                    while (temp.Next != pivot)
                        temp = temp.Next;
                    temp.Next = null;

                    newHead = QuickSort(newHead, temp);
                    temp = GetTail(newHead);
                    temp.Next = pivot;
                }

                pivot.Next = QuickSort(pivot.Next, newTail);

                return newHead;
            }

            // Partition function (Rearranges the list around the pivot)
            private Node Partition(Node start, Node end, ref Node newHead, ref Node newTail)
            {
                Node pivot = end;
                Node prev = null, current = start, tail = pivot;

                while (current != pivot)
                {
                    if (current.Date < pivot.Date)
                    {
                        if (newHead == null)
                            newHead = current;

                        prev = current;
                        current = current.Next;
                    }
                    else
                    {
                        if (prev != null)
                            prev.Next = current.Next;

                        Node temp = current.Next;
                        current.Next = null;
                        tail.Next = current;
                        tail = current;
                        current = temp;
                    }
                }

                if (newHead == null)
                    newHead = pivot;

                newTail = tail;
                return pivot;
}




        // Displays all the room records.
        public void Show()
        {
            Node? ptr = head;
            if (ptr == null)
            {
                Console.WriteLine("\nNo records to show.");
                return;
            }
            while (ptr != null)
            {
                Console.WriteLine("\nRoom ID: " + ptr.Id);
                Console.WriteLine("Customer Name: " + (ptr.Name ?? "Not Booked"));
                Console.WriteLine("Booking Date: " + (ptr.Date == DateTime.MinValue ? "Not Booked" : ptr.Date.ToString("yyyy-MM-dd")));
                Console.WriteLine("Room Type: " + ptr.RoomType);
                Console.WriteLine("Available: " + (ptr.IsAvailable ? "Yes" : "No"));
                ptr = ptr.Next;
            }
        }

        // Save to CSV using CsvHelper
        public void SaveToCSV(string filePath = "hotel_data.csv")
        {
            using var writer = new StreamWriter(filePath);
            using var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                Delimiter = ","
            });
            
                // Write headers
                csv.WriteHeader<NodeData>();
                csv.NextRecord();

                Node? ptr = head;
                while (ptr != null)
                {
                    csv.WriteRecord(new NodeData
                    {
                        Id = ptr.Id,
                        Date = ptr.Date,
                        Name = ptr.Name,
                        RoomType = ptr.RoomType,
                        IsAvailable = ptr.IsAvailable
                    });
                    //csv.WriteRecord(nodeData);
                    csv.NextRecord();
                    ptr = ptr.Next;
                }
            
            Console.WriteLine("Data saved to CSV file.");
        }

        // Load from CSV using CsvHelper
        public void LoadFromCSV(string filePath = "hotel_data.csv")
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine("No CSV data found.");
                return;
            }

            head = null;
            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                Delimiter = ",",
                MissingFieldFound = null,
                TrimOptions = TrimOptions.Trim
            });

            csv.Context.TypeConverterCache.AddConverter<DateTime>(new CustomDateTimeConverter());
            var records = csv.GetRecords<NodeData>().ToList();

            foreach (var record in records)
            {
                var newNode = new Node
                {
                    Id = record.Id,
                    Date = record.Date,
                    Name = record.Name,
                    RoomType = record.RoomType,
                    IsAvailable = record.IsAvailable,
                    Next = null
                };

                if (head == null) head = newNode;
                else
                {
                    var ptr = head;
                    while (ptr.Next != null) ptr = ptr.Next;
                    ptr.Next = newNode;
                }
            }
            Console.WriteLine("Data loaded from CSV.");
        }

        private class NodeData
        {
            public int Id { get; set; }
            public DateTime Date { get; set; }
            public string? Name { get; set; }
            public string? RoomType { get; set; }
            public bool IsAvailable { get; set; }
        }

        private class CustomDateTimeConverter : DefaultTypeConverter
        {
            public override object? ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
            {
                if (string.IsNullOrWhiteSpace(text) || text.ToLower() == "not booked")
                    return DateTime.MinValue;

                if (DateTime.TryParseExact(text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                    return date;

                if (DateTime.TryParse(text, out date))
                    return date;

                throw new FormatException($"Invalid date format: '{text}'");
            }

            public override string? ConvertToString(object? value, IWriterRow row, MemberMapData memberMapData)
            {
                if (value is DateTime date && date == DateTime.MinValue)
                    return "Not Booked";

                return base.ConvertToString(value, row, memberMapData);
            }
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Hotel hotel = new Hotel();
            hotel.Menu();
        }
    }
}