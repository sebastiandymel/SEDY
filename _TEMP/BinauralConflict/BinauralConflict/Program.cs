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
            var decisionMaker = new DecisionMaker();
            var trimmerDecision = decisionMaker.Factory.CreateTrimmerConflictDecision(null, StrictSide.Left);

            var decisions = new List<BinauralConflictDecision>();
            var handler = new BinauralConflictHandler();
            foreach (var decision in decisions)
            {
                decision.Accept(handler);
            }
        }
    }

    /// <summary>
    /// Represents interface for handling binaural conflicts
    /// </summary>
    public interface IBinauralConflictHandler
    {
        /// <summary>
        /// Execute conflict handler to resolve it according to given decision
        /// </summary>
        /// <param name="decision">Chosen decision how to handle given conflict</param>
        void Execute(ProgramMismatchDecision decision);
        /// <summary>
        /// Trimmer specific decision
        /// </summary>
        /// <param name="decision"></param>
        void Execute(TrimmerConflictDecision decision);
    }

    public class BinauralConflictHandler : IBinauralConflictHandler
    {
        public void Execute(ProgramMismatchDecision decision)
        {
            
        }

        public void Execute(TrimmerConflictDecision decision)
        {
            
        }
    }

    public abstract class BinauralConflictDecision
    {
        /// <summary>
        /// Conflict to handle
        /// </summary>
        public abstract SettingsType Conflict { get; }
        /// <summary>
        /// Resolution side
        /// </summary>
        public StrictSide? Side { get; set; }

        public abstract void Accept(IBinauralConflictHandler handler);
    }

    public class TrimmerConflictDecision: BinauralConflictDecision
    {
        public override SettingsType Conflict => SettingsType.Trimmer;
        /// <summary>
        /// Trimmer paths to handle
        /// </summary>
        public string[] Paths { get; set; }

        public override void Accept(IBinauralConflictHandler handler)
        {
            handler.Execute(this);
        }
    }

    public class ProgramMismatchDecision : BinauralConflictDecision
    {
        public override SettingsType Conflict => SettingsType.ProgramMismatch;

        public override void Accept(IBinauralConflictHandler handler)
        {
            throw new NotImplementedException();
        }
    }

    public interface ICanAcceptConflictHandler
    {
        void Accept(IBinauralConflictHandler handler);
    }

    public class DecisionMaker
    {
        public DecisionFactory Factory { get; } = new DecisionFactory();
    }

    public class DecisionFactory
    {
        public BinauralConflictDecision CreateProgramMismatchDecision(StrictSide side)
        {
            return new ProgramMismatchDecision();
        }

        public BinauralConflictDecision CreateTrimmerConflictDecision(string[] paths, StrictSide side)
        {
            return new TrimmerConflictDecision();
        }
    }


    public enum StrictSide
    {
        Left,
        Right
    }

    public enum SettingsType
    {
        ProgramMismatch,
        Trimmer
    }
}
