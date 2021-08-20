// First and Last Name: Ilya Zavidny
// Date: 20.08.2021
// Time taken: 2 hours 17 minutes
// I was not sure whether the serialised format should be human readable,
// so I decided that it will not be

using System;
using System.IO;
using System.Collections.Generic;


class ListNode {

    public ListNode Previous;
    public ListNode Next;
    public ListNode Random; // random element in list
    public string Data;
    
    public ListNode(string data) {
        Data = data;
    }
}
class ListRandom {
    public ListNode Head;
    public ListNode Tail;
    public int Count;
    Random rand = new Random();

    public ListNode this[int i] {
        get {
            if ( i >= Count || i < 0)
                throw new IndexOutOfRangeException();
            ListNode curNode = Head;
            while ( i > 0 ) {
                curNode = curNode.Next;
                i--;
            }
            return curNode;
        }

        set {
            if ( i >= Count || i < 0)
                throw new IndexOutOfRangeException();
            ListNode curNode = Head;
            while ( i > 0 ) {
                curNode = curNode.Next;
                i--;
            }
            curNode = value;
        }
    }

    public void Add(string data) {
        ListNode newNode = new ListNode(data);
        if (Head == null) {
            Head = newNode;
            Tail = newNode;
        } else {
            Tail.Next = newNode;
            newNode.Previous = Tail;
            Tail = newNode;
        }
        Count++;
        newNode.Random = this[rand.Next(Count)];
    }


    // BinaryWriter is used because it makes code shorter,
    // since we dont have to do conversions ourselves and handle string length
    // (binary write outputs string with prefixed length which is then read by reader)
    public void Serialize(Stream s) {
        using (BinaryWriter bw = new BinaryWriter(s)) {
            ListNode curNode = Head;
            Dictionary<ListNode, int> randId = new Dictionary<ListNode, int>();
            while (curNode != null){
                randId[curNode] = randId.Count;
                curNode = curNode.Next;
            }
            curNode = Head;
            bw.Write(Count);
            while (curNode != null) {
                bw.Write(curNode.Data);
                bw.Write(randId[curNode.Random]);
                curNode = curNode.Next;
            }
        }
    }

    public void Deserialize(Stream s) {
        int[] randIds;
        int curI = 0;
        Dictionary<int, ListNode> randNodes = new Dictionary<int, ListNode>();
        using( BinaryReader br = new BinaryReader(s)) {
            randIds = new int[br.ReadInt32()];
            while(br.PeekChar() != -1) {
                Add(br.ReadString());
                randIds[curI] = br.ReadInt32();
                randNodes[curI++] = Tail;
            }
        }
        ListNode curNode = Head;
        curI = 0;
        while (curNode != null) {
            curNode.Random = randNodes[randIds[curI++]];
            curNode = curNode.Next;
        }
    }

    public void Print() {
        for (ListNode curNode = Head; curNode != null; curNode = curNode.Next) {
            Console.Write("Data: " + curNode.Data + '\t');
            Console.Write("Random: " + 
                           (curNode.Random != null ? curNode.Random.Data : "Null") + '\t');
            Console.WriteLine("\n**************");
        }
    }
}


class Program {
    static void PrintMessage(string msg) {
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.Write(msg);
        Console.ResetColor();
    }
    static void Main() {
        ListRandom lr = new ListRandom();
        lr.Add("Quake Champions");
        lr.Add("Crysis Remastered");
        lr.Add("World War Z");
        lr.Add("Halo Online");
        PrintMessage("Initial List:\n");
        lr.Print();
        PrintMessage("Serialising List...\n");
        lr.Serialize(File.Open("Saber", FileMode.OpenOrCreate));
        PrintMessage("List was serialised into file \"Saber\"\n");
        ListRandom dl = new ListRandom();
        dl.Deserialize(File.Open("Saber", FileMode.Open));
        PrintMessage("Deserialised list:\n");
        dl.Print();
   }
}
