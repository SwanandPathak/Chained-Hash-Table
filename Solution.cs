/* 
 * Solution.cs 
 * 
 * Version: 1.0 
 *  
 */
using System;
using System.Collections.Generic;
/** 
 * Implementation of a Chained Hash Table 
 * 
 * @author      Swanand Pathak 
 *
 */
namespace RIT_CS
{
        /** 
         * Throws exception of value of Key not present in the hash table asked to fetch.
         * 
         */
   
    public class NonExistentKey<Key> : Exception
    {
        public Key BadKey { get; private set; }
        public NonExistentKey(Key k) :
            base("Non existent key in HashTable: " + k)
        {
            BadKey = k;
        }

    }
    /** 
     * Interface Table with put(),contains() and Get() FUnctions.
     * 
     */
    interface Table<Key, Value> : IEnumerable<Key>
    {
        void Put(Key k, Value v);
        bool Contains(Key k);
        Value Get(Key k);
    }
 /** 
 *  Class TableEntry - For Each Enter Will have a Associative Key-Value Pair. 
 * 
 * 
 *
 */
    class TableEntry<Key, Value>
    {
        public Key key { get; private set; }
        public Value value;
        public TableEntry(Key k, Value v)
        {
            this.key = k;
            this.value = v;
        }
    }
    /** 
 * Class TableImplementation Implements the interface Table (Implementing Class)
 * 
 *  
 *
 */

    class TableImplementation<Key, Value> : Table<Key, Value>
    {
        private List<List<TableEntry<Key, Value>>> aHashtable;      // List of List to store Chained Hash Table
        public int capacity { get; private set; }                   // Capacity is the current size of hash Table
        private double loadThreshold;                               // It is the Ratio of number of entries in the Hash table to its Capacity
        public int noOfEntries { get; private set; }                // No of key-Value pair entries to the Hash Table

        /** 
         * Constructor of TableImplementation
         * assigns capacity,loadThreashold,noOfEntries values for an instance of Table
         * 
         */

        public TableImplementation(int capacity, double loadThreshold)
        {
            aHashtable = new List<List<TableEntry<Key, Value>>>(capacity);
            for (int i = 0; i < capacity; i++)
                aHashtable.Add(new List<TableEntry<Key, Value>>());
            this.capacity = capacity;
            this.loadThreshold = loadThreshold;
            this.noOfEntries = 0;
        }

        /** 
         * Function to add a new Key-Value Pair to the Hash Table 
         * @param k - Key value
         * @param v - Value associated with key
         * 
         * @return null
         */
        public void Put(Key k, Value v)
        {

            int index;                                                  //To store the bucket Index
            index = Math.Abs(k.GetHashCode() % (capacity));             //Getting bucket value 
            int entryPresent = 0;

            foreach (TableEntry<Key, Value> t in aHashtable[index])
            {
                if (t.key.Equals(k))                                    // Storing the Key-Value pair
                {
                    t.value = v;
                    entryPresent = 1;                                   // if key Already exists
                }

            }

            if (entryPresent == 0)
            {
                aHashtable[index].Add(new TableEntry<Key, Value>(k, v));         // if does not
                noOfEntries++;
            }

            if ((noOfEntries / capacity) > loadThreshold)               // If Threshold exceeds then rehash
            {
                rehashing();
            }

        }
        /** 
         * Function returns true if an entry with the given key exists, else returns false
         * 
         * @param k - Key value to be checked
         * 
         * @return bool
         */

        public bool Contains(Key k)
        {
            int index;
            index = Math.Abs(k.GetHashCode() % capacity);
            foreach (TableEntry<Key, Value> e in aHashtable[index])
            {
                if (e.key.Equals(k))
                {
                    return true;

                }

            }
            return false;
        }

        /** 
         * Function to give back the value associated with a certain Key.
         * 
         * @param k - Key value to be fetched
         * 
         * @return Value v
         * 
         */
        public Value Get(Key k)
        {
            int index;
            index = Math.Abs(k.GetHashCode() % capacity);
            foreach (TableEntry<Key, Value> e in aHashtable[index])
            {
                if (e.key.Equals(k))                                        // Checking Key k value
                    return e.value;
            }
            throw new NonExistentKey<Key>(k);                               // If key not found throw nonexistentkey exception
        }

        /** 
         * Generic GetEnumerator Function, used to give enumerator for Hash table 
         * 
         */

        public IEnumerator<Key> GetEnumerator()
        {
            foreach (List<TableEntry<Key, Value>> l in aHashtable)
            {
                if (l.Count != 0)
                {
                    foreach (TableEntry<Key, Value> e in l)
                        yield return e.key;
                }
            }

        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /** 
         * Function to implement rehashing once (no of entries/ capacity) ratio exceeds loadThreshold
         * 
         */
        private void rehashing()
        {
            capacity += (int)(capacity * 0.5);                                                          //increase capacity by 50%
            List<List<TableEntry<Key, Value>>> bHashtable = new List<List<TableEntry<Key, Value>>>(capacity);    
            for (int i = 0; i < capacity; i++)
                bHashtable.Add(new List<TableEntry<Key, Value>>());

            foreach (List<TableEntry<Key, Value>> l in aHashtable)
            {
                foreach (TableEntry<Key, Value> e in l)
                {
                    int index = Math.Abs(e.key.GetHashCode() % capacity);                               //ReHash All entries using new capacity value and store in b
                    bHashtable[index].Add(new TableEntry<Key, Value>(e.key, e.value));
                }
            }

            aHashtable = bHashtable;                                                                                      // Reassign b to a

        }
    }
    /** 
      * Formation of hash table with given or set capacity and threshold values
      * 
      */
    class TableFactory
    {
        public static Table<K, V> Make<K, V>(int capacity = 100, double loadThreshold = 0.75)
        {
            return new TableImplementation<K, V>(capacity, loadThreshold);
        }
    }

    class Test
    {
        /** 
         * Various tests on the hash table.
         * 
         */

        public static void test()
        {
            //Capacity Test

            Table<String, String> ht = TableFactory.Make<String, String>(4, 0.5);
            int j = 1,max=10000;
            for (int i = 0; i < max; i++)
            {
                ht.Put("" + i, "" + j);
                j++;                                                                //Testing the Hash Table by adding ten thousand elements to see its capacity handling

            }
            if(j==(max+1))
            {
                Console.WriteLine("Capacity Test Successful");
            }
            else
            {
                Console.WriteLine("Capacity Test UnSuccessful");
             }
            

            // Generic Test
            Table<String,int> ht1 = TableFactory.Make<String,int>(4, 0.5);
            String s = "a";
            int max1 = 10;
            for (int i = 0; i < max1; i++)                                          //Test the hash Table by adding different generics.....here <String,int> 
            {
                s = s + "s";
                ht1.Put("" + s, i);


            }
            if(ht1.Get(s)==max1-1)
            {
                Console.WriteLine("Generic Test Successful");
            }
            else
            {
                Console.WriteLine("Generic Test Unsuccesssful");
            }


            // Key not present test
            Table<String, String> ht2 = TableFactory.Make<String, String>(4, 0.5);
            String s1 = "b";
            for(int i=0;i<10;i++)                                                       // Adding 0-9 as keys..and finding value for Key=b, if throws exception then successful
            {
                ht2.Put(""+i, s1+"c");                                                                                                                                                                                                              
                s1 = s1 + "a";
            }
            Console.WriteLine("Key not present Test Successful if throws Exception");
            Console.WriteLine(ht2.Get("b"));



           
          }
           
            
       }

        /** 
         * Main class
         * @param String args[] null
         * 
         */
    class MainClass
    {
        public static void Main(string[] args)
        {
            Table<String, String> ht = TableFactory.Make<String, String>(4, 0.5);
            ht.Put("Joe", "Doe");
            ht.Put("Jane", "Brain");
            ht.Put("Chris", "Swiss");
            try
            {
                foreach (String first in ht)
                {
                    Console.WriteLine(first + " -> " + ht.Get(first));
                }
                Console.WriteLine("=========================");

                ht.Put("Wavy", "Gravy");
                ht.Put("Chris", "Bliss");
                foreach (String first in ht)
                {
                    Console.WriteLine(first + " -> " + ht.Get(first));
                }
                Console.WriteLine("=========================");

                Console.Write("Jane -> ");
                Console.WriteLine(ht.Get("Jane"));
                Test.test();                                                    //Calling Test Method
            }
            catch (NonExistentKey<String> nek)
            {
                Console.WriteLine(nek.Message);
                Console.WriteLine(nek.StackTrace);
            }
          
        
           
            Console.ReadLine();
        }
    }
}