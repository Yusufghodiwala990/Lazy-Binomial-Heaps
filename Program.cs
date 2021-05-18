/*----------------------------------------------------------------------*/

/* This console application implements a Lazy Binomial Heap. The Code for some methods like BinomialLink was taken from Dr Patricks
 * Implementation of a standard Binomial Heap. Lazy Binomial Heaps allows for faster ammortized time complexity of operations
 * due to insertions and Front() taking O(1) time. Remove cleans up the 'mess' left behind by insert and combines trees of same size
 * But over time even Remove() at most will take O(log n)
 * 

/*----------------------------------------------------------------------*/

//Assignment 3 : PART 2, Lazy Binomial Heap by Yusuf Ghodiwala (0683640)


    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;




    // Using some of the methods of Binomial Heap by Dr Patrick to write a Lazy Binomial Heap solution
    namespace BinomialHeap
    {
        public class BinomialNode<T>
        {
            public T Item { get; set; }
            public int Degree { get; set; }
            public BinomialNode<T> LeftMostChild { get; set; }
            public BinomialNode<T> RightSibling { get; set; }

            // Constructor

            public BinomialNode(T item)
            {
                Item = item;
                Degree = 0;
                LeftMostChild = null;
                RightSibling = null;
            }
        }

        //--------------------------------------------------------------------------------------

        // Common interface for all non-linear data structures

        public interface IContainer<T>
        {
            void MakeEmpty();  // Reset an instance to empty
            bool Empty();      // Test if an instance is empty
            int Size();        // Return the number of items in an instance
        }

        //--------------------------------------------------------------------------------------

        public interface IBinomialHeap<T> : IContainer<T> where T : IComparable
        {
            void Add(T item);               // Add an item to a binomial heap
            void Remove();                  // Remove the item with the highest priority
            T Front();                      // Return the item with the highest priority
           
        }

        //--------------------------------------------------------------------------------------

        // Binomial Heap
        // Implementation:  Leftmost-child, right-sibling

        public class BinomialHeap<T> : IBinomialHeap<T> where T : IComparable
        {
            private BinomialNode<T> head;  // Head of the root list
            private int size;              // Size of the binomial heap

            // redefining a heap by creating an array of binomial trees which stores trees of the same size at A[k] where k is 2^k.
            private BinomialNode<T>[] A;   
            private BinomialNode<T> MaxPr;    // new data member -  ref to the root with the max priority
       
        
       
      
            // Contructor
            

            public BinomialHeap()
            {
                A = new BinomialNode<T>[15];        // creating a heap which can store up to 2^15 items


                // intializing each 'bucket' with a header node
                for (int i = 0; i < A.Length; i++)
                    A[i] = new BinomialNode<T>(default(T));

               head = new BinomialNode<T>(default(T));   // Header node
               size = 0;
            }

            
            
            // Method : Add
            // Parameters: Takes a item passed by the user
            // Time complexity:  O(1)  // no merge required
            // Return Type : void
            // Description: Adds an item to A[0] bucket as a binomial tree of size 0.
                      
            public void Add(T item)
            {

                
                BinomialNode<T> n = new BinomialNode<T>(item);

                // assigning the new item in the array[0] of Binomial Trees size 0  
                // all new items get attached there.

                n.RightSibling = A[0].RightSibling;                         
                A[0].RightSibling = n;



            // this check will only pass the first time an item is added


                if (MaxPr == null)                    // MaxPr will be null when the first item has just been inserted
                    MaxPr = A[0].RightSibling;       //  then MaxPr will become the first item in Heap at A[0]
            

                // updating MaxPr if the new item just added has higher priority than the current MaxPr

                if (MaxPr != null && MaxPr.Item.CompareTo(A[0].RightSibling.Item) < 0)
                    MaxPr = A[0].RightSibling;                             // new items are added to the front of the root list
                                                                            // at A[0]



                // increase size
                size++;
       
            }



            // Method : Remove
            // Parameters: Takes a item passed by the user
            // Ammortized Time complexity:  O(log n) 
            // Return Type : void
            // Description: First removes an item, creates a heap of the children of the removed item, assigns a respective
            //              'bucket' for the children based on their size of Binomial Trees, and then calls coalesce to combine
            //               all trees of the same size in appropriate buckets

            public void Remove()
            {

                // if the heap is not empty
                if (!Empty())
                {
                    BinomialHeap<T> H = new BinomialHeap<T>();           // this heap will store the removed item's children

                    BinomialNode<T> p, q;                                 // references to store the the Binomial Tree of the
                                                                          //     removed item(root) 


                    // curr will be used to traverse and find MaxPr in the heap          
                    BinomialNode<T> curr = new BinomialNode<T>(default(T));  

                    bool found = false;
                    
                    // looping through the array(buckets) and it's binomial trees to remove MaxPr.
                    for (int i = 0; i < A.Length; i++)
                    {
                        curr = A[i];            // assigning curr to be the header node of each bucket of size i.
                        

                        // traversing that bucket's rootlist to find MaxPr
                        while (curr.RightSibling!=null && !found)
                        {

                            if (curr.RightSibling == MaxPr)
                                found = true;                            
                            else                        
                                curr = curr.RightSibling;
        
                        }


                        if (found)     // if we found MaxPr, no need to look for it in the other buckets
                            break;
                    }


                    p = curr.RightSibling;                               // p will be assigned to MaxPr

                    curr.RightSibling = curr.RightSibling.RightSibling;  // deleting MaxPr by referencing around it in the root list.

                    


                    p = p.LeftMostChild;                                  // move down to the leftmostchild of MaxPr

                    // Add binomial subtrees of p in reverse order into H
                    while (p != null)
                    {
                        q = p.RightSibling;

                        // Splice p into H as the first binomial tree
                        p.RightSibling = H.head.RightSibling;
                        H.head.RightSibling = p;


                        p = q;
                        H.size++;        // increase the size of heap which contains the children of MaxPr.
                    }

                    size--;       // decrease the size of the overall heap because we removed an item

                    

                    // placing the new heap's binomial trees into it's apt position in the array
                    BinomialNode<T> childTree;
                    childTree = H.head.RightSibling;
                    

                    // if MaxPr's tree was size 2^k, after the removal, the children will be of size 2^k-1
                    //   2^k-1 = 2^0, 2^1, 2^2..... 2^k-1.

                    // So we traverse from 2^0 to 2^k-1 and add all the children into appropriate buckets of that size.
                    for (int i = 0; i < H.size; i++)
                    {

                        BinomialNode<T> packet = childTree;        // packet will contain the binomial tree of size i in the children
                        childTree = childTree.RightSibling;          // move on to the next size of binomial tree in the children
                 


                        packet.RightSibling = A[i].RightSibling;  // assigning packet to size i in the array
                        A[i].RightSibling = packet;

                        
                    }


                    // calling coalesce to combine trees of the same size in the array
                    Coalesce(ref A);

                    calcMax();   // recalculating MaxPr (i asked Dr Patrick, it wouldn't hurt the Time Complexity after a Coalesce)
                    
                }
             
            }


            public void calcMax()
            {

                int k;

       // The first loop will point MaxPr to the first available item of a tree 
                //(available means the bucket must have at least one tree)

                for (k = 0; k < A.Length; k++)                 
                {
                    if (A[k].RightSibling == null)     // if the bucket is empty, skip it
                        continue;


                    MaxPr = A[k].RightSibling;  // found the first available item
                    break;              // exit loop
                }
                

                // Continue iterating where the first loop left off to compare with other available trees/items
                for (int i = k + 1; i < A.Length; i++)
                {
                    // if there is no binomial tree in bucket of size i or the bucket has not been initialized 
                      if (A[i].RightSibling == null)      
                        continue;                                      // skip it
                    


                    // Compare the current item with MaxPr
                    if(MaxPr.Item.CompareTo(A[i].RightSibling.Item) < 0)
                        MaxPr = A[i].RightSibling;



                }

                return;



            }
            // Front
            // Returns the item with the highest priority
            // Time complexity:  O(1) // due to MaxPr

            public T Front()
            {
            
                if (!Empty())
                {
                    return MaxPr.Item;
                }
                else
                    return default(T);
            }



            public void Coalesce(ref BinomialNode<T> [] A)
            {


                // Iterating each bucket of size k
                for (int k = 0; k < A.Length; k++)
                {
                    

                   
                    BinomialNode<T> curr = A[k];  // assigning curr to be at the header of each bucket

                   
                    // if there are at least 2 trees still in the current index or 'bucket'
                    // Since we're at index k, those two trees will be of the same size
                    while (curr.RightSibling!=null && curr.RightSibling.RightSibling != null)
                    {

                        // store the two trees into tree1 and 2
                        BinomialNode<T>tree1 = curr.RightSibling;
                        BinomialNode<T>tree2 = curr.RightSibling.RightSibling;

                      
                        // delete the two trees from that Array[i] or bucket
                        A[k].RightSibling = A[k].RightSibling.RightSibling.RightSibling;

                      



                        // combining two trees and creating a new one
                        BinomialNode<T> newTree;

                        // compare both trees' roots priority and calling BinomialLink method to move the root to leftmostchild
                        //    depending on which root had a higher priority
                        if (tree1.Item.CompareTo(tree2.Item) < 0)
                            newTree = BinomialLink(tree1, tree2);          
                        else
                            newTree = BinomialLink(tree2, tree1);

                       
                        

                        // placing the new combined tree into bucket  k + 1.
                        // e.g 2^2 + 2^2 = 2^3
                        newTree.RightSibling = A[k+1].RightSibling;
                        A[k+1].RightSibling = newTree;

                       

                                                 
                    }


                }



            }
           

            // BinomialLink
            // Makes child the leftmost child of root
            // Time complexity:  O(1)

            private BinomialNode<T> BinomialLink(BinomialNode<T> child, BinomialNode<T> root)
            {
                child.RightSibling = root.LeftMostChild;
                root.LeftMostChild = child;
                
                return root;
            }

          




            public void print()
            {
                int i;
                for (i = 0; i < A.Length; i++)  
                {
                    if (A[i].RightSibling == null)
                    {
                        Console.WriteLine(" Tree of Size 2^{0} : Empty", i);
                        Console.WriteLine();
                        continue;  // skip the iteration
                    }

                    // else call private print to print the binomial tree of size 2^i.
                    Console.WriteLine("Tree of Size 2^{0}", i);
                    print(A[i].RightSibling, 0); 
                    Console.WriteLine("\n"); 
                }
            }


            private void print(BinomialNode<T> tree, int indent)
            {
                if (tree == null)  //if the tree is null
                    return;
                
                else  
                {
                    if (tree.RightSibling != null)   // if a rightsibling does exist
                        print(tree.RightSibling, indent);  //recursively visit it
                    
                    Console.WriteLine(new String(' ', indent) + tree.Item);     // print the current item in order

                    if (tree.LeftMostChild != null)   //if a leftmostchild does exist
                        print(tree.LeftMostChild, indent + 5);  //recusrively visit it.
                }

            }

            // MakeEmpty
            // Creates an empty binomial heap
            // Time complexity:  O(1)

            public void MakeEmpty()
            {
                A = null;
                size = 0;
            }

            // Empty
            // Returns true is the binomial heap is empty; false otherwise
            // Time complexity:  O(1)

            public bool Empty()
            {
                return size == 0;
            }

            // Size
            // Returns the number of items in the binomial heap
            // Time complexity:  O(1)

            public int Size()
            {
                return size;
            }

            
        }

        //--------------------------------------------------------------------------------------

        // Used by class BinomailHeap<T>
        // Implements IComparable and overrides ToString (from Object)

        public class PriorityClass : IComparable
        {
            private int priorityValue;
            private char letter;

            public PriorityClass(int priority, char letter)
            {
                this.letter = letter;
                priorityValue = priority;
            }

            public int CompareTo(Object obj)
            {
                PriorityClass other = (PriorityClass)obj;   // Explicit cast
                return priorityValue - other.priorityValue;  // High values have higher priority
            }

            public override string ToString()
            {
                return letter.ToString() + "-" + priorityValue;
            }
        }



    public class Test
        {
            public static void Main(string[] args)
            {
                int i;
                Random r = new Random();

               BinomialHeap<PriorityClass> BH = new BinomialHeap<PriorityClass>();



               Console.WriteLine("Inserting 6 items in the Heap");
               Console.WriteLine();

                // Testing with 6 items, and printing the heap
                for (i=0; i<6; i++)
                { 
                    BH.Add(new PriorityClass(r.Next(50), (char)('a')));
                }


                Console.WriteLine("Highest Priority Item is : {0}", BH.Front());
                BH.print();





                Console.WriteLine("Highest Priority Item is : {0}", BH.Front());
                // Removing 1 item. (Highest Priority)
                BH.Remove();

               
                Console.WriteLine("After 1 removal, this is the Tree");
                BH.print();
                Console.WriteLine("Highest Priority Item is : {0}", BH.Front());




                Console.WriteLine("Printing the Tree before 2nd Removal");
                BH.print();
                Console.WriteLine();

                Console.WriteLine("Highest Priority Item is : {0}", BH.Front());
                // Removing second item. (Highest Priority)
                BH.Remove();


                Console.WriteLine("After 2 removals, this is the Tree");
                BH.print();
                Console.WriteLine("Highest Priority Item is : {0}", BH.Front());






                // Testing with 20 items

                BinomialHeap<PriorityClass> BN = new BinomialHeap<PriorityClass>();
                for (i = 0; i < 20; i++)
                {
                    BN.Add(new PriorityClass(r.Next(50), (char)('a')));
                }



                Console.WriteLine("Before 1 removal in heap of 20 items");
                Console.WriteLine("Highest Priority Item is : {0}", BN.Front());
                BN.print();

                Console.WriteLine();


                


                Console.WriteLine("After 1 removal in a heap of 20 items, this is the Tree");
                BN.Remove();
                BN.print();
                Console.WriteLine("Highest Priority Item is : {0}", BN.Front());






                Console.WriteLine("Before 2 removals in heap of 20 items");
                Console.WriteLine("Highest Priority Item is : {0}", BN.Front());
                BN.print();

                Console.WriteLine();





                Console.WriteLine("After 2 removals in a heap of 20 items, this is the Tree");
                BN.Remove();
                BN.print();
                Console.WriteLine("Highest Priority Item is : {0}", BN.Front());




                Console.WriteLine("Before 3 removals in heap of 20 items");
                Console.WriteLine("Highest Priority Item is : {0}", BN.Front());
                BN.print();

                Console.WriteLine();





                Console.WriteLine("After 3 removals in a heap of 20 items, this is the Tree");
                BN.Remove();
                BN.print();
                Console.WriteLine("Highest Priority Item is : {0}", BN.Front());









                // creating another instance and removing 5 items at once

              

                BinomialHeap<PriorityClass> BP = new BinomialHeap<PriorityClass>();
                for (i = 0; i < 20; i++)
                {
                    BP.Add(new PriorityClass(r.Next(50), (char)('a')));
                }


                Console.WriteLine("Before 5 consecutive removals in a new heap of 20 items");
                Console.WriteLine("Highest Priority Item is : {0}", BP.Front());
                BP.print();


                for (i = 0; i < 5; i++)
                    BP.Remove();



                Console.WriteLine("After 5 consecutive removals in a new heap of 20 items");
                Console.WriteLine("Highest Priority Item is : {0}", BP.Front());
                BP.print();

                Console.WriteLine();
                



                Console.WriteLine("Size after 5 removals from a heap of 20 items : {0}", BP.Size());


                Console.WriteLine("Current Highest Priority Item is : {0}", BP.Front());

                Console.ReadLine();
            }
        }
    }