// Skeleton implementation written by Joe Zachary for CS 3500, January 2015.
// Revised for CS 3500 by Bernie Cosgriff, February 4, 2016

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Collections.ObjectModel;

namespace Dependencies
{
    /// <summary>
    /// A DependencyGraph can be modeled as a set of dependencies, where a dependency is an ordered 
    /// pair of strings.  Two dependencies (s1,t1) and (s2,t2) are considered equal if and only if 
    /// s1 equals s2 and t1 equals t2.
    /// 
    /// Given a DependencyGraph DG:
    /// 
    ///    (1) If s is a string, the set of all strings t such that the dependency (s,t) is in DG 
    ///    is called the dependents of s, which we will denote as dependents(s).
    ///        
    ///    (2) If t is a string, the set of all strings s such that the dependency (s,t) is in DG 
    ///    is called the dependees of t, which we will denote as dependees(t).
    ///    
    /// The notations dependents(s) and dependees(s) are used in the specification of the methods of this class.
    ///
    /// For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}
    ///     dependents("a") = {"b", "c"}
    ///     dependents("b") = {"d"}
    ///     dependents("c") = {}
    ///     dependents("d") = {"d"}
    ///     dependees("a") = {}
    ///     dependees("b") = {"a"}
    ///     dependees("c") = {"a"}
    ///     dependees("d") = {"b", "d"}
    ///     
    /// All of the methods below require their string parameters to be non-null.  This means that 
    /// the behavior of the method is undefined when a string parameter is null.  
    ///
    /// IMPORTANT IMPLEMENTATION NOTE
    /// 
    /// The simplest way to describe a DependencyGraph and its methods is as a set of dependencies, 
    /// as discussed above.
    /// 
    /// However, physically representing a DependencyGraph as, say, a set of ordered pairs will not
    /// yield an acceptably efficient representation.  DO NOT USE SUCH A REPRESENTATION.
    /// 
    /// You'll need to be more clever than that.  Design a representation that is both easy to work
    /// with as well acceptably efficient according to the guidelines in the PS3 writeup. Some of
    /// the test cases with which you will be graded will create massive DependencyGraphs.  If you
    /// build an inefficient DependencyGraph this week, you will be regretting it for the next month.
    /// </summary>
    public class DependencyGraph
    {
        // Dictionarys that will contain the dependents and dependees
        // of the DependecyGraph. A HashSet of dependents/dependees for each node 
        // will be stored in their repective Dictionarys using the node's name as the key.
        // No empty lists will be stored in either dictionary. If a list becomes empty it
        // will be removed from its respective dictionary.
        private Dictionary<string, HashSet<string>> dependents;
        private Dictionary<string, HashSet<string>> dependees;

        // The number of nodes in the graph.
        private int size;


        /// <summary>
        /// Creates a DependencyGraph containing no dependencies.
        /// </summary>
        public DependencyGraph()
        {
            //Initialize all of our instance variables.
            dependents = new Dictionary<string, HashSet<string>>();
            dependees = new Dictionary<string, HashSet<string>>();
            size = 0;
        }

        /// <summary>
        /// Creates a new DependencyGraph that is equivalent to dg.
        /// Throws an ArgumentNullException if dg is null.
        /// </summary>
        /// <param name="dg"></param>
        public DependencyGraph(DependencyGraph dg)
        {
            dependees = new Dictionary<string, HashSet<string>>();
            dependents = new Dictionary<string, HashSet<string>>();
            if(dg == null)
            {
                throw new ArgumentNullException();
            }
            
            //// Set the dictionarys and size to a copy of dg's dictionarys and size
            //dependees = new Dictionary<string, HashSet<string>>(dg.dependees);
            //dependents = new Dictionary<string, HashSet<string>>(dg.dependents);
            //size = dg.Size;

            foreach (KeyValuePair<string, HashSet<string>> pair in dg.dependees)
            {
                dependees.Add(pair.Key, new HashSet<string>(pair.Value));
            }
            foreach (KeyValuePair<string, HashSet<string>> pair in dg.dependents)
            {
                dependents.Add(pair.Key, new HashSet<string>(pair.Value));
            }
            size = dg.Size;
        }

        /// <summary>
        /// The number of dependencies in the DependencyGraph.
        /// </summary>
        public int Size
        {
            get { return size; }
        }

        /// <summary>
        /// Reports whether dependents(s) is non-empty. Throws
        /// an ArgumentNullException if s == null.
        /// </summary>
        public bool HasDependents(string s)
        {
            if(s == null)
            {
                throw new ArgumentNullException();
            }

            //Return whether there is a list for s or not.
            return dependents.ContainsKey(s);
        }

        /// <summary>
        /// Reports whether dependees(s) is non-empty. Throws
        /// an ArgumentNullException if s == null.
        /// </summary>
        public bool HasDependees(string s)
        {
            if (s == null)
            {
                throw new ArgumentNullException();
            }

            //Return whether there is a list for s or not.
            return dependees.ContainsKey(s);


        }

        /// <summary>
        /// Enumerates dependents(s).  Throws
        /// an ArgumentNullException if s == null.
        /// </summary>
        public IEnumerable<string> GetDependents(string s)
        {
            if (s == null)
            {
                throw new ArgumentNullException();
            }

            HashSet<string> set;

            //If there is a list for s return a copy of it
            if (dependents.TryGetValue(s, out set))
            {
                return new HashSet<string>(set);
            }
            //Else just return an empty set
            else
            {
                return new HashSet<string>();
            }
        }

        /// <summary>
        /// Enumerates dependees(s).  Throws
        /// an ArgumentNullException if s == null.
        /// </summary>
        public IEnumerable<string> GetDependees(string s)
        {
            if (s == null)
            {
                throw new ArgumentNullException();
            }
            
            HashSet<string> set;

            //If there is a list for s return a copy of it
            if (dependees.TryGetValue(s, out set))
            {
                return new HashSet<string>(set);
            }
            //Else just return an empty set
            else
            {
                return new HashSet<string>();
            }
        }

        /// <summary>
        /// Adds the dependency (s,t) to this DependencyGraph.
        /// This has no effect if (s,t) already belongs to this DependencyGraph.
        /// Throws an ArgumentNullException if s == null or t == null.
        /// </summary>
        public void AddDependency(string s, string t)
        {
            if (s == null || t == null)
            {
                throw new ArgumentNullException();
            }

            // Try and add a dependency and increase the size if successful
            if (AddDependee(t, s) && AddDependent(s, t))
            {
                size++;
            }
        }

        /// <summary>
        /// Removes the dependency (s,t) from this DependencyGraph.
        /// Does nothing if (s,t) doesn't belong to this DependencyGraph.
        /// Throws an ArgumentNullException if s == null or t == null.
        /// </summary>
        public void RemoveDependency(string s, string t)
        {
            if (s == null || t == null)
            {
                throw new ArgumentNullException();
            }

            // Try to remove a dependency and decrease the size if successful
            if (RemoveDependee(t, s) && RemoveDependent(s, t))
            {
                size--;
            }
        }

        /// <summary>
        /// Removes the dependee s from the set t.
        /// Returns true if the set t is altered,
        /// otherwise returns false.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        private bool RemoveDependee(string t, string s)
        {
            bool decreaseSize = false;
            HashSet<string> set;

            //If there is a list for t and s is in it, remove s from it
            if (dependees.TryGetValue(t, out set))
            {
                decreaseSize = set.Remove(s);

                if (set.Count == 0)
                {
                    dependees.Remove(t);
                }
            }
            return decreaseSize;
        }


        /// <summary>
        /// Removes the dependent t from the set s.
        /// Returns true if the set s is altered,
        /// otherwise returns false.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        private bool RemoveDependent(string s, string t)
        {
            bool decreaseSize = false;
            HashSet<string> set;

            //If there is a list for s and t is in it, remove t from it
            if (dependents.TryGetValue(s, out set))
            {
                decreaseSize = set.Remove(t);

                if (set.Count == 0)
                {
                    dependents.Remove(s);
                }
            }
            return decreaseSize;
        }

        /// <summary>
        /// Removes all existing dependencies of the form (s,r).  Then, for each
        /// t in newDependents, adds the dependency (s,t).
        /// Throws an ArgumentNullException if s == null or newDependents == null.
        /// </summary>
        public void ReplaceDependents(string s, IEnumerable<string> newDependents)
        {
            if (s == null || newDependents == null)
            {
                throw new ArgumentNullException();
            }

            HashSet<string> set;

            // If there is a set for s, decrease size by the set's size,
            // remove s from each string str's set of dependees,
            // and then remove that set from the dependents dictionary.
            if (dependents.TryGetValue(s, out set))
            {
                size -= set.Count;
                foreach(string str in set)
                {
                    if (String.IsNullOrEmpty(str))
                    {
                        throw new ArgumentNullException();
                    }
                    RemoveDependee(str, s);
                }
                dependents.Remove(s);
            }

            // Make a new set out of newDependents
            set = new HashSet<string>(newDependents);

            // If that list is not empty increase size by list's size,
            // add s to each string str's set of dependees,
            // and add it to the dependents dictionary.
            if (set.Count != 0)
            {
                foreach(string str in newDependents)
                {
                    if(String.IsNullOrEmpty(str))
                    {
                        throw new ArgumentNullException();
                    }
                    AddDependee(str, s);
                }
                size += set.Count;
                dependents.Add(s, set);
            }
        }

        /// <summary>
        /// Removes all existing dependencies of the form (r,t).  Then, for each 
        /// s in newDependees, adds the dependency (s,t).
        /// Throws ArgumentNullException if t == null or newDependees == null.
        /// </summary>
        public void ReplaceDependees(string t, IEnumerable<string> newDependees)
        {
            if (newDependees == null || t == null)
            {
                throw new ArgumentNullException();
            }
            
            HashSet<string> set;

            // If there is a set for s, decrease size by the set's size,
            // remove s from each string str's set of dependents,
            // and then remove that set from the dependees dictionary.
            if (dependees.TryGetValue(t, out set))
            {
                size -= set.Count;
                foreach (string str in set)
                {
                    if (String.IsNullOrEmpty(str))
                    {
                        throw new ArgumentNullException();
                    }
                    RemoveDependent(str, t);
                }
                dependees.Remove(t);
            }

            // Make a new set out of newDependents
            set = new HashSet<string>(newDependees);

            // If that set is not empty increase size by set's size,
            // add s to each string str's set of dependents,
            // and add it to the dependees dictionary.
            if (set.Count != 0)
            {
                foreach (string str in newDependees)
                {
                    if (String.IsNullOrEmpty(str))
                    {
                        throw new ArgumentNullException();
                    }
                    AddDependent(str, t);
                }
                size += set.Count;
                dependees.Add(t, set);
            }
        }

        /// <summary>
        /// Adds s to the set t of dependees.
        /// Returns true if the set t is altered,
        /// otherwise returns false.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        private bool AddDependee(string t, string s)
        {
            bool added = true;
            HashSet<string> set;

            // If there is a set for t and it does
            // not contain s, add s
            if (dependees.TryGetValue(t, out set))
            {
                added = set.Add(s);
            }

            // Otherwise make a new set, add s,
            // and add the set to the dictionary
            else
            {
                dependees.Add(t, new HashSet<string>() { s });
            }
            return added;
        }

        /// <summary>
        /// Adds t to the set s of dependents.
        /// Returns true if the set s is altered,
        /// otherwise returns false.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        private bool AddDependent(string s, string t)
        {
            bool added = true;
            HashSet<string> set;


            // If there is a set for s and it does
            // not contain t, add t
            if (dependents.TryGetValue(s, out set))
            {
                added = set.Add(t);
            }
            // Otherwise make a new set, add t,
            // and add the set to the dictionary
            else
            {
                dependents.Add(s, new HashSet<string>() { t });
            }
            return added;
        }
    }
}
