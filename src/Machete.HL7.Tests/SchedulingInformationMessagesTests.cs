namespace Machete.HL7.Tests
{
    using System;
    using HL7Schema.V26;
    using NUnit.Framework;
    using Testing;

    [TestFixture]
    public class SchedulingQueryTests :
        HL7MacheteTestHarness<MSH, HL7Entity>
    {
        [Test]
        public void S01_NewAppointment_message_test()
        {
            const string message
= @"MSH|^~\&|SENDING APP|SENDING FAC|EPIC|EPIC|2005080710350000||SRM^S01|1919|P|2.3||
ARQ|1|119953350||||1^SCH|||60|MIN|200510141500||||||||593^RUCHALA^JOANNA^^^^|(608)238-5653|||
NTE|1||The patient has a stomach ache.
NTE|2||The patient watches 12 hours of TV a day.
PID|||556345||SMITH^GEORGE^^^^|PALOMA^BONNY^J.^^^|19720124|M|BOWMAN^JEFFREY^^^^~PALOMA^PICASSO^^^^|B|1255 MAIN St^ROOM 12^Whitefish Bay^WI^53217^^^|MILWAUKEE|962-3222|258-9515|ENGLISH|M|METHODIST||323-44-4456|R240-4272-3576-09|||^^Orlando^FL|||||||N
OBX|1|TX|||The patient gets 6 hours of sleep a night.
OBX|2|TX|||The patient takes aspirin for a heart problem.
DG1|1||002.0^TYPHOID FEVER^I9
RGS|1||1^INTERNAL MEDICINE
AIS|1||1^OFFICE VISIT
NTE|1||The patient should drink 8 glasses of water a day.
NTE|2||The patient should get 20 mins of exercise 3 times a week.
AIG|1||39^White^ROOM^^^|||||200510141500|0|MIN|15|MIN
NTE|1||ROOM White is a room with an MRI scan machine.
NTE|2||It can be used in half hour intervals.
AIG|2||41^ROOM^ELMO^^^|||||200510141515|0|MIN|15|MIN
NTE|1||The Elmo room is for kids.
NTE|2||The Elmo room is soft and fuzzy.
AIP|1||38^BARKER^BILL^^^|||200510141515|0|MIN|15|MIN
NTE|1||Dr. Barker speaks Latin and Greek.
NTE|2||Dr. Barker has been a doctor for 15 years
RGS|2||5^RADIOLOGY
AIS|1||1^OFFICE VISIT
AIG|1||43^XRAY^SEVEN^^^|||||200510141530|0|MIN|30|MIN
NTE|1||XRAY SEVEN cannot be booked for patients weighing over 200 pounds.
AIP|1||42^CRICKET,JOANNA^^^|||200510141545|0|MIN|15|MIN
NTE|1||Dr. Cricket has latex allergy.";

            ParseResult<HL7Entity> parse = Parser.Parse(message);

            var result = parse.Query(q =>
            {
                var arqQuery = from arq in q.Select<ARQ>()
                               from nte in q.Select<NTE>().ZeroOrMore()
                               //from pid in q.Select<PID>().ZeroOrMore()
                               select new
                               {
                                   ARQ = arq,
                                   NTE = nte,
                                   //PID = pid
                               };

                var pidQuery = from pid in q.Select<PID>().ZeroOrMore()
                               from pv1 in q.Select<PV1>().ZeroOrMore()
                               from pv2 in q.Select<PV2>().ZeroOrMore()
                               from obx in q.Select<OBX>().ZeroOrMore()
                               from dg1 in q.Select<DG1>().ZeroOrMore()
                               select new
                               {
                                   PID = pid,
                                   PV1 = pv1,
                                   PV2 = pv2,
                                   OBX = obx,
                                   DG1 = dg1
                               };

                //var obrQuery = from obr in q.Select<OBR>()
                //               from dg1 in q.Select<DG1>().Optional()
                //               from obx in obxQuery.Optional()
                //               select new
                //               {
                //                   OBR = obr,
                //                   DG1 = dg1,
                //                   OBX = obx
                //               };

                //var testQuery = from orc in q.Select<ORC>()
                //                from obr in obrQuery.ZeroOrMore()
                //                select new
                //                {
                //                    ORC = orc,
                //                    OBR = obr
                //                };

                return from msh in q.Select<MSH>()
                       from nte in q.Select<NTE>().ZeroOrMore()
                           //from skip in q.Except<HL7Segment, ORC>().ZeroOrMore()
                       from arq in arqQuery.One()
                       from pid in pidQuery.OneOrMore()
                       select new
                       {
                           MSH = msh,
                           Notes = nte,
                           Arq = arq,
                           Pid = pid
                       };
            });

            Assert.That(result.HasResult, Is.True);
            Assert.AreEqual(1, result.Result.Arq.Count);
            Assert.AreEqual(2, result.Result.Arq[0].NTE.Count);
            var apptId = result.Result.Arq[0].ARQ.FillerAppointmentId.Value.EntityIdentifier.ValueOrDefault();
            Assert.That(apptId, Is.EqualTo("119953350"));
            Assert.AreEqual(1, result.Result.Pid.Count);
            Assert.AreEqual(2, result.Result.Pid[0].OBX.Count);
            Assert.AreEqual(1, result.Result.Pid[0].DG1.Count);
            var patiendSSN = result.Result.Pid[0].PID[0].SSNNumberPatient.Value;
            Assert.That(patiendSSN, Is.EqualTo("323-44-4456"));
        }
    }
}
