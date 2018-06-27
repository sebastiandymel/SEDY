using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinauralConflict
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }

    /// <summary>
    /// Represents interface for handling binaural conflicts
    /// </summary>
    public interface IBinauralConflictHandler
    {
        /// <summary>
        /// Determines if given decision can be handled
        /// </summary>
        /// <param name="decision">Chosen decision how to handle given conflict</param>        
        bool CanHandle(BinauralConflictDecision decision);
        /// <summary>
        /// Execute conflict handler to resolve it according to given decision
        /// </summary>
        /// <param name="decision">Chosen decision how to handle given conflict</param>
        void Execute(BinauralConflictDecision decision);
    }

    public class BinauralConflictDecision
    {
        /// <summary>
        /// Conflict to handle
        /// </summary>
        public SettingsType Conflict { get; set; }
        /// <summary>
        /// Resolution side
        /// </summary>
        public StrictSide? Side { get; set; }
    }

    public class TrimmerConflictDecision: BinauralConflictDecision
    {
        /// <summary>
        /// Trimmer paths to handle
        /// </summary>
        public string[] Paths { get; set; }
    }



    public enum StrictSide
    {
        Left,
        Right
    }

    public enum SettingsType
    {
        ProgramMismatch
    }
}
