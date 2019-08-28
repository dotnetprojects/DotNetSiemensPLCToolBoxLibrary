using DotNetSiemensPLCToolBoxLibrary.General;
using NUnit.Framework;
using System.Xml;

namespace ToolBoxLibUnitTests
{
	[TestFixture]
	public class TestXPath
	{
		[Test]
		public void TestCRC16BlockSum()
		{

            var test = @"<?xml version=""1.0"" encoding=""utf-8""?>
<Document>
  <Engineering version=""V15.1"" />
  <SW.Blocks.FB ID=""0"">
    <AttributeList>
      <AutoNumber>false</AutoNumber>
      <HandleErrorsWithinBlock ReadOnly=""true"">false</HandleErrorsWithinBlock>
      <HeaderAuthor />
      <HeaderFamily />
      <HeaderName />
      <HeaderVersion>0.1</HeaderVersion>
      <Interface>
        <Sections xmlns=""http://www.siemens.com/automation/Openness/SW/Interface/v3"">
          <Section Name=""Input"" />
          <Section Name=""Output"" />
          <Section Name=""InOut"" />
          <Section Name=""Static"">
            <Member Name=""SystemErrorClass"" Datatype=""Int"" Remanence=""SetInIDB"" Accessibility=""Public"">
              <AttributeList>
                <BooleanAttribute Name=""ExternalAccessible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalVisible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalWritable"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""SetPoint"" SystemDefined=""true"">false</BooleanAttribute>
              </AttributeList>
              <Comment>
                <MultiLanguageText Lang=""de-DE"">Stoerung Anlage 3=keine Stoerung</MultiLanguageText>
              </Comment>
            </Member>
            <Member Name=""AushubLageSpeicher"" Datatype=""Array[1..2] of DInt"" Remanence=""SetInIDB"" Accessibility=""Public"">
              <AttributeList>
                <BooleanAttribute Name=""ExternalAccessible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalVisible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalWritable"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""SetPoint"" SystemDefined=""true"">false</BooleanAttribute>
              </AttributeList>
            </Member>
            <Member Name=""AushubZeitSpeicher"" Datatype=""Array[1..2] of Int"" Remanence=""SetInIDB"" Accessibility=""Public"">
              <AttributeList>
                <BooleanAttribute Name=""ExternalAccessible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalVisible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalWritable"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""SetPoint"" SystemDefined=""true"">false</BooleanAttribute>
              </AttributeList>
            </Member>
            <Member Name=""FWAktivZeitSpeicher"" Datatype=""Int"" Remanence=""SetInIDB"" Accessibility=""Public"">
              <AttributeList>
                <BooleanAttribute Name=""ExternalAccessible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalVisible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalWritable"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""SetPoint"" SystemDefined=""true"">false</BooleanAttribute>
              </AttributeList>
            </Member>
            <Member Name=""HWAktivZeitSpeicher"" Datatype=""Array[1..2] of Int"" Remanence=""SetInIDB"" Accessibility=""Public"">
              <AttributeList>
                <BooleanAttribute Name=""ExternalAccessible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalVisible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalWritable"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""SetPoint"" SystemDefined=""true"">false</BooleanAttribute>
              </AttributeList>
            </Member>
            <Member Name=""TG11AktivZeitSpeicher"" Datatype=""Int"" Remanence=""SetInIDB"" Accessibility=""Public"">
              <AttributeList>
                <BooleanAttribute Name=""ExternalAccessible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalVisible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalWritable"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""SetPoint"" SystemDefined=""true"">false</BooleanAttribute>
              </AttributeList>
            </Member>
            <Member Name=""TG12AktivZeitSpeicher"" Datatype=""Int"" Remanence=""SetInIDB"" Accessibility=""Public"">
              <AttributeList>
                <BooleanAttribute Name=""ExternalAccessible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalVisible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalWritable"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""SetPoint"" SystemDefined=""true"">false</BooleanAttribute>
              </AttributeList>
            </Member>
            <Member Name=""TG21AktivZeitSpeicher"" Datatype=""Int"" Remanence=""SetInIDB"" Accessibility=""Public"">
              <AttributeList>
                <BooleanAttribute Name=""ExternalAccessible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalVisible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalWritable"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""SetPoint"" SystemDefined=""true"">false</BooleanAttribute>
              </AttributeList>
            </Member>
            <Member Name=""TG22AktivZeitSpeicher"" Datatype=""Int"" Remanence=""SetInIDB"" Accessibility=""Public"">
              <AttributeList>
                <BooleanAttribute Name=""ExternalAccessible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalVisible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalWritable"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""SetPoint"" SystemDefined=""true"">false</BooleanAttribute>
              </AttributeList>
            </Member>
            <Member Name=""RF11AktivZeitSpeicher"" Datatype=""Int"" Remanence=""SetInIDB"" Accessibility=""Public"">
              <AttributeList>
                <BooleanAttribute Name=""ExternalAccessible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalVisible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalWritable"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""SetPoint"" SystemDefined=""true"">false</BooleanAttribute>
              </AttributeList>
            </Member>
            <Member Name=""RF12AktivZeitSpeicher"" Datatype=""Int"" Remanence=""SetInIDB"" Accessibility=""Public"">
              <AttributeList>
                <BooleanAttribute Name=""ExternalAccessible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalVisible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalWritable"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""SetPoint"" SystemDefined=""true"">false</BooleanAttribute>
              </AttributeList>
            </Member>
            <Member Name=""RF21AktivZeitSpeicher"" Datatype=""Int"" Remanence=""SetInIDB"" Accessibility=""Public"">
              <AttributeList>
                <BooleanAttribute Name=""ExternalAccessible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalVisible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalWritable"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""SetPoint"" SystemDefined=""true"">false</BooleanAttribute>
              </AttributeList>
            </Member>
            <Member Name=""RF22AktivZeitSpeicher"" Datatype=""Int"" Remanence=""SetInIDB"" Accessibility=""Public"">
              <AttributeList>
                <BooleanAttribute Name=""ExternalAccessible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalVisible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalWritable"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""SetPoint"" SystemDefined=""true"">false</BooleanAttribute>
              </AttributeList>
            </Member>
            <Member Name=""SU01AktivZeitSpeicher"" Datatype=""Int"" Remanence=""SetInIDB"" Accessibility=""Public"">
              <AttributeList>
                <BooleanAttribute Name=""ExternalAccessible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalVisible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalWritable"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""SetPoint"" SystemDefined=""true"">false</BooleanAttribute>
              </AttributeList>
            </Member>
            <Member Name=""SU02AktivZeitSpeicher"" Datatype=""Int"" Remanence=""SetInIDB"" Accessibility=""Public"">
              <AttributeList>
                <BooleanAttribute Name=""ExternalAccessible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalVisible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalWritable"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""SetPoint"" SystemDefined=""true"">false</BooleanAttribute>
              </AttributeList>
            </Member>
            <Member Name=""CU01AktivZeitSpeicher"" Datatype=""Int"" Remanence=""SetInIDB"" Accessibility=""Public"">
              <AttributeList>
                <BooleanAttribute Name=""ExternalAccessible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalVisible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalWritable"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""SetPoint"" SystemDefined=""true"">false</BooleanAttribute>
              </AttributeList>
            </Member>
            <Member Name=""CU02AktivZeitSpeicher"" Datatype=""Int"" Remanence=""SetInIDB"" Accessibility=""Public"">
              <AttributeList>
                <BooleanAttribute Name=""ExternalAccessible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalVisible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalWritable"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""SetPoint"" SystemDefined=""true"">false</BooleanAttribute>
              </AttributeList>
            </Member>
            <Member Name=""SF1AktivZeitSpeicher"" Datatype=""Int"" Remanence=""SetInIDB"" Accessibility=""Public"">
              <AttributeList>
                <BooleanAttribute Name=""ExternalAccessible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalVisible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalWritable"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""SetPoint"" SystemDefined=""true"">false</BooleanAttribute>
              </AttributeList>
            </Member>
            <Member Name=""SF2AktivZeitSpeicher"" Datatype=""Int"" Remanence=""SetInIDB"" Accessibility=""Public"">
              <AttributeList>
                <BooleanAttribute Name=""ExternalAccessible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalVisible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalWritable"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""SetPoint"" SystemDefined=""true"">false</BooleanAttribute>
              </AttributeList>
            </Member>
            <Member Name=""EX1AktivZeitSpeicher"" Datatype=""Int"" Remanence=""SetInIDB"" Accessibility=""Public"">
              <AttributeList>
                <BooleanAttribute Name=""ExternalAccessible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalVisible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalWritable"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""SetPoint"" SystemDefined=""true"">false</BooleanAttribute>
              </AttributeList>
            </Member>
            <Member Name=""EX2AktivZeitSpeicher"" Datatype=""Int"" Remanence=""SetInIDB"" Accessibility=""Public"">
              <AttributeList>
                <BooleanAttribute Name=""ExternalAccessible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalVisible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalWritable"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""SetPoint"" SystemDefined=""true"">false</BooleanAttribute>
              </AttributeList>
            </Member>
            <Member Name=""KF11AktivZeitSpeicher"" Datatype=""Int"" Remanence=""SetInIDB"" Accessibility=""Public"">
              <AttributeList>
                <BooleanAttribute Name=""ExternalAccessible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalVisible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalWritable"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""SetPoint"" SystemDefined=""true"">false</BooleanAttribute>
              </AttributeList>
            </Member>
            <Member Name=""KF12AktivZeitSpeicher"" Datatype=""Int"" Remanence=""SetInIDB"" Accessibility=""Public"">
              <AttributeList>
                <BooleanAttribute Name=""ExternalAccessible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalVisible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalWritable"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""SetPoint"" SystemDefined=""true"">false</BooleanAttribute>
              </AttributeList>
            </Member>
            <Member Name=""KF21AktivZeitSpeicher"" Datatype=""Int"" Remanence=""SetInIDB"" Accessibility=""Public"">
              <AttributeList>
                <BooleanAttribute Name=""ExternalAccessible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalVisible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalWritable"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""SetPoint"" SystemDefined=""true"">false</BooleanAttribute>
              </AttributeList>
            </Member>
            <Member Name=""KF22AktivZeitSpeicher"" Datatype=""Int"" Remanence=""SetInIDB"" Accessibility=""Public"">
              <AttributeList>
                <BooleanAttribute Name=""ExternalAccessible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalVisible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalWritable"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""SetPoint"" SystemDefined=""true"">false</BooleanAttribute>
              </AttributeList>
            </Member>
            <Member Name=""FehlerFC123FW"" Datatype=""Byte"" Remanence=""SetInIDB"" Accessibility=""Public"">
              <AttributeList>
                <BooleanAttribute Name=""ExternalAccessible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalVisible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalWritable"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""SetPoint"" SystemDefined=""true"">false</BooleanAttribute>
              </AttributeList>
            </Member>
            <Member Name=""FehlerFC123HW"" Datatype=""Byte"" Remanence=""SetInIDB"" Accessibility=""Public"">
              <AttributeList>
                <BooleanAttribute Name=""ExternalAccessible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalVisible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalWritable"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""SetPoint"" SystemDefined=""true"">false</BooleanAttribute>
              </AttributeList>
            </Member>
            <Member Name=""AutoResetDeleteOrderRP11"" Datatype=""TON_TIME"" Version=""1.0"" Remanence=""SetInIDB"" Accessibility=""Public"">
              <AttributeList>
                <BooleanAttribute Name=""ExternalAccessible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalVisible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalWritable"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""SetPoint"" SystemDefined=""true"">false</BooleanAttribute>
              </AttributeList>
              <Sections>
                <Section Name=""None"">
                  <Member Name=""PT"" Datatype=""Time"">
                    <AttributeList>
                    </AttributeList>
                  </Member>
                  <Member Name=""ET"" Datatype=""Time"">
                    <AttributeList>
                    </AttributeList>
                  </Member>
                  <Member Name=""IN"" Datatype=""Bool"">
                    <AttributeList>
                    </AttributeList>
                  </Member>
                  <Member Name=""Q"" Datatype=""Bool"">
                    <AttributeList>
                    </AttributeList>
                  </Member>
                </Section>
              </Sections>
            </Member>
            <Member Name=""AutoResetDeleteOrderRP12"" Datatype=""TON_TIME"" Version=""1.0"" Remanence=""SetInIDB"" Accessibility=""Public"">
              <AttributeList>
                <BooleanAttribute Name=""ExternalAccessible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalVisible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalWritable"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""SetPoint"" SystemDefined=""true"">false</BooleanAttribute>
              </AttributeList>
              <Sections>
                <Section Name=""None"">
                  <Member Name=""PT"" Datatype=""Time"">
                    <AttributeList>
                    </AttributeList>
                  </Member>
                  <Member Name=""ET"" Datatype=""Time"">
                    <AttributeList>
                    </AttributeList>
                  </Member>
                  <Member Name=""IN"" Datatype=""Bool"">
                    <AttributeList>
                    </AttributeList>
                  </Member>
                  <Member Name=""Q"" Datatype=""Bool"">
                    <AttributeList>
                    </AttributeList>
                  </Member>
                </Section>
              </Sections>
            </Member>
            <Member Name=""AutoResetDeleteOrderRP21"" Datatype=""TON_TIME"" Version=""1.0"" Remanence=""SetInIDB"" Accessibility=""Public"">
              <AttributeList>
                <BooleanAttribute Name=""ExternalAccessible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalVisible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalWritable"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""SetPoint"" SystemDefined=""true"">false</BooleanAttribute>
              </AttributeList>
              <Sections>
                <Section Name=""None"">
                  <Member Name=""PT"" Datatype=""Time"">
                    <AttributeList>
                    </AttributeList>
                  </Member>
                  <Member Name=""ET"" Datatype=""Time"">
                    <AttributeList>
                    </AttributeList>
                  </Member>
                  <Member Name=""IN"" Datatype=""Bool"">
                    <AttributeList>
                    </AttributeList>
                  </Member>
                  <Member Name=""Q"" Datatype=""Bool"">
                    <AttributeList>
                    </AttributeList>
                  </Member>
                </Section>
              </Sections>
            </Member>
            <Member Name=""HandAuftragZeitSpeicher"" Datatype=""Array[1..2, 1..2] of Int"" Remanence=""SetInIDB"" Accessibility=""Public"">
              <AttributeList>
                <BooleanAttribute Name=""ExternalAccessible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalVisible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalWritable"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""SetPoint"" SystemDefined=""true"">false</BooleanAttribute>
              </AttributeList>
            </Member>
            <Member Name=""SondAuftragZeitSpeicher"" Datatype=""Array[1..2, 1..2] of Int"" Remanence=""SetInIDB"" Accessibility=""Public"">
              <AttributeList>
                <BooleanAttribute Name=""ExternalAccessible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalVisible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalWritable"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""SetPoint"" SystemDefined=""true"">false</BooleanAttribute>
              </AttributeList>
            </Member>
            <Member Name=""AutoAuftragZeitSpeicher"" Datatype=""Array[1..2, 1..2] of Int"" Remanence=""SetInIDB"" Accessibility=""Public"">
              <AttributeList>
                <BooleanAttribute Name=""ExternalAccessible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalVisible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalWritable"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""SetPoint"" SystemDefined=""true"">false</BooleanAttribute>
              </AttributeList>
            </Member>
            <Member Name=""CamTriggerLeft"" Datatype=""&quot;CamTrigger&quot;"" Accessibility=""Public"">
              <AttributeList>
                <BooleanAttribute Name=""ExternalAccessible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalVisible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalWritable"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""SetPoint"" SystemDefined=""true"">false</BooleanAttribute>
              </AttributeList>
              <Sections>
                <Section Name=""Input"">
                  <Member Name=""Execute"" Datatype=""Bool"">
                    <AttributeList>
                    </AttributeList>
                  </Member>
                  <Member Name=""Init"" Datatype=""Bool"">
                    <AttributeList>
                    </AttributeList>
                  </Member>
                </Section>
                <Section Name=""Output"" />
                <Section Name=""InOut"">
                  <Member Name=""Config"" Datatype=""TCON_IP_v4"" Version=""1.0"">
                    <AttributeList>
                    </AttributeList>
                    <Sections>
                      <Section Name=""None"" />
                    </Sections>
                  </Member>
                </Section>
                <Section Name=""Static"">
                  <Member Name=""Connect"" Datatype=""Bool"">
                    <AttributeList>
                    </AttributeList>
                  </Member>
                  <Member Name=""SendRequest"" Datatype=""Bool"">
                    <AttributeList>
                    </AttributeList>
                  </Member>
                  <Member Name=""CamTrig"" Datatype=""R_TRIG"" Version=""1.0"">
                    <Sections>
                      <Section Name=""Input"">
                        <Member Name=""CLK"" Datatype=""Bool"" />
                      </Section>
                      <Section Name=""Output"">
                        <Member Name=""Q"" Datatype=""Bool"" />
                      </Section>
                      <Section Name=""InOut"" />
                      <Section Name=""Static"">
                        <Member Name=""Stat_Bit"" Datatype=""Bool"" />
                      </Section>
                    </Sections>
                  </Member>
                  <Member Name=""message"" Datatype=""String[7]"">
                    <AttributeList>
                    </AttributeList>
                  </Member>
                  <Member Name=""SendData"" Datatype=""Array[0..4000] of Byte"">
                    <AttributeList>
                    </AttributeList>
                  </Member>
                  <Member Name=""TSEND_C_Instance"" Datatype=""TSEND_C"" Version=""3.2"">
                    <Sections>
                      <Section Name=""Input"">
                        <Member Name=""REQ"" Datatype=""Bool"" />
                        <Member Name=""CONT"" Datatype=""Bool"" />
                        <Member Name=""LEN"" Datatype=""UDInt"" />
                      </Section>
                      <Section Name=""Output"">
                        <Member Name=""DONE"" Datatype=""Bool"" />
                        <Member Name=""BUSY"" Datatype=""Bool"" />
                        <Member Name=""ERROR"" Datatype=""Bool"" />
                        <Member Name=""STATUS"" Datatype=""Word"" />
                      </Section>
                      <Section Name=""InOut"">
                        <Member Name=""CONNECT"" Datatype=""Variant"" />
                        <Member Name=""DATA"" Datatype=""Variant"" />
                        <Member Name=""ADDR"" Datatype=""Variant"" />
                        <Member Name=""COM_RST"" Datatype=""Bool"" />
                      </Section>
                      <Section Name=""Static"">
                        <Member Name=""s_state"" Datatype=""Int"" />
                        <Member Name=""s_locked"" Datatype=""Bool"" />
                        <Member Name=""s_udp"" Datatype=""Bool"" />
                        <Member Name=""s_configured"" Datatype=""Bool"" />
                        <Member Name=""s_tcon_80A3"" Datatype=""Bool"" />
                        <Member Name=""s_REQ"" Datatype=""Bool"" />
                        <Member Name=""s_ConID"" Datatype=""CONN_OUC"" />
                        <Member Name=""s_TCON"" Datatype=""TCON"" Version=""4.0"">
                          <Sections>
                            <Section Name=""Input"">
                              <Member Name=""REQ"" Datatype=""Bool"" />
                              <Member Name=""ID"" Datatype=""CONN_OUC"" />
                            </Section>
                            <Section Name=""Output"">
                              <Member Name=""DONE"" Datatype=""Bool"" />
                              <Member Name=""BUSY"" Datatype=""Bool"" />
                              <Member Name=""ERROR"" Datatype=""Bool"" />
                              <Member Name=""STATUS"" Datatype=""Word"" />
                            </Section>
                            <Section Name=""InOut"">
                              <Member Name=""CONNECT"" Datatype=""Variant"" />
                            </Section>
                            <Section Name=""Static"" />
                          </Sections>
                        </Member>
                        <Member Name=""s_TDIAG"" Datatype=""T_DIAG"" Version=""1.2"">
                          <Sections>
                            <Section Name=""Input"">
                              <Member Name=""REQ"" Datatype=""Bool"" />
                              <Member Name=""ID"" Datatype=""CONN_OUC"" />
                            </Section>
                            <Section Name=""Output"">
                              <Member Name=""DONE"" Datatype=""Bool"" />
                              <Member Name=""BUSY"" Datatype=""Bool"" />
                              <Member Name=""ERROR"" Datatype=""Bool"" />
                              <Member Name=""STATUS"" Datatype=""Word"" />
                            </Section>
                            <Section Name=""InOut"">
                              <Member Name=""RESULT"" Datatype=""Variant"" />
                            </Section>
                            <Section Name=""Static"" />
                          </Sections>
                        </Member>
                        <Member Name=""s_TDIAG_Status"" Datatype=""TDiag_Status"" Version=""1.0"">
                          <Sections>
                            <Section Name=""None"">
                              <Member Name=""InterfaceId"" Datatype=""HW_ANY"" />
                              <Member Name=""ID"" Datatype=""CONN_OUC"" />
                              <Member Name=""ConnectionType"" Datatype=""Byte"" />
                              <Member Name=""ActiveEstablished"" Datatype=""Bool"" />
                              <Member Name=""State"" Datatype=""Byte"" />
                              <Member Name=""Kind"" Datatype=""Byte"" />
                              <Member Name=""SentBytes"" Datatype=""UDInt"" />
                              <Member Name=""ReceivedBytes"" Datatype=""UDInt"" />
                            </Section>
                          </Sections>
                        </Member>
                        <Member Name=""s_TDISCON"" Datatype=""TDISCON"" Version=""2.1"">
                          <Sections>
                            <Section Name=""Input"">
                              <Member Name=""REQ"" Datatype=""Bool"" />
                              <Member Name=""ID"" Datatype=""CONN_OUC"" />
                            </Section>
                            <Section Name=""Output"">
                              <Member Name=""DONE"" Datatype=""Bool"" />
                              <Member Name=""BUSY"" Datatype=""Bool"" />
                              <Member Name=""ERROR"" Datatype=""Bool"" />
                              <Member Name=""STATUS"" Datatype=""Word"" />
                            </Section>
                            <Section Name=""InOut"" />
                            <Section Name=""Static"" />
                          </Sections>
                        </Member>
                        <Member Name=""s_TSEND"" Datatype=""TSEND"" Version=""4.0"">
                          <Sections>
                            <Section Name=""Input"">
                              <Member Name=""REQ"" Datatype=""Bool"" />
                              <Member Name=""ID"" Datatype=""CONN_OUC"" />
                              <Member Name=""LEN"" Datatype=""UDInt"" />
                            </Section>
                            <Section Name=""Output"">
                              <Member Name=""DONE"" Datatype=""Bool"" />
                              <Member Name=""BUSY"" Datatype=""Bool"" />
                              <Member Name=""ERROR"" Datatype=""Bool"" />
                              <Member Name=""STATUS"" Datatype=""Word"" />
                            </Section>
                            <Section Name=""InOut"">
                              <Member Name=""DATA"" Datatype=""Variant"" />
                              <Member Name=""ADDR"" Datatype=""Variant"" />
                            </Section>
                            <Section Name=""Static"" />
                          </Sections>
                        </Member>
                        <Member Name=""s_TUSEND"" Datatype=""TUSEND"" Version=""4.0"">
                          <Sections>
                            <Section Name=""Input"">
                              <Member Name=""REQ"" Datatype=""Bool"" />
                              <Member Name=""ID"" Datatype=""CONN_OUC"" />
                              <Member Name=""LEN"" Datatype=""UDInt"" />
                            </Section>
                            <Section Name=""Output"">
                              <Member Name=""DONE"" Datatype=""Bool"" />
                              <Member Name=""BUSY"" Datatype=""Bool"" />
                              <Member Name=""ERROR"" Datatype=""Bool"" />
                              <Member Name=""STATUS"" Datatype=""Word"" />
                            </Section>
                            <Section Name=""InOut"">
                              <Member Name=""DATA"" Datatype=""Variant"" />
                              <Member Name=""ADDR"" Datatype=""Variant"" />
                            </Section>
                            <Section Name=""Static"" />
                          </Sections>
                        </Member>
                        <Member Name=""s_TRESET"" Datatype=""T_RESET"" Version=""1.2"">
                          <Sections>
                            <Section Name=""Input"">
                              <Member Name=""REQ"" Datatype=""Bool"" />
                              <Member Name=""ID"" Datatype=""CONN_OUC"" />
                            </Section>
                            <Section Name=""Output"">
                              <Member Name=""DONE"" Datatype=""Bool"" />
                              <Member Name=""BUSY"" Datatype=""Bool"" />
                              <Member Name=""ERROR"" Datatype=""Bool"" />
                              <Member Name=""STATUS"" Datatype=""Word"" />
                            </Section>
                            <Section Name=""InOut"" />
                            <Section Name=""Static"" />
                          </Sections>
                        </Member>
                      </Section>
                    </Sections>
                  </Member>
                  <Member Name=""TSEND_C_Error"" Datatype=""Word"">
                    <AttributeList>
                    </AttributeList>
                  </Member>
                </Section>
              </Sections>
            </Member>
            <Member Name=""CamTriggerRight"" Datatype=""&quot;CamTrigger&quot;"" Accessibility=""Public"">
              <AttributeList>
                <BooleanAttribute Name=""ExternalAccessible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalVisible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalWritable"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""SetPoint"" SystemDefined=""true"">false</BooleanAttribute>
              </AttributeList>
              <Sections>
                <Section Name=""Input"">
                  <Member Name=""Execute"" Datatype=""Bool"">
                    <AttributeList>
                    </AttributeList>
                  </Member>
                  <Member Name=""Init"" Datatype=""Bool"">
                    <AttributeList>
                    </AttributeList>
                  </Member>
                </Section>
                <Section Name=""Output"" />
                <Section Name=""InOut"">
                  <Member Name=""Config"" Datatype=""TCON_IP_v4"" Version=""1.0"">
                    <AttributeList>
                    </AttributeList>
                    <Sections>
                      <Section Name=""None"" />
                    </Sections>
                  </Member>
                </Section>
                <Section Name=""Static"">
                  <Member Name=""Connect"" Datatype=""Bool"">
                    <AttributeList>
                    </AttributeList>
                  </Member>
                  <Member Name=""SendRequest"" Datatype=""Bool"">
                    <AttributeList>
                    </AttributeList>
                  </Member>
                  <Member Name=""CamTrig"" Datatype=""R_TRIG"" Version=""1.0"">
                    <Sections>
                      <Section Name=""Input"">
                        <Member Name=""CLK"" Datatype=""Bool"" />
                      </Section>
                      <Section Name=""Output"">
                        <Member Name=""Q"" Datatype=""Bool"" />
                      </Section>
                      <Section Name=""InOut"" />
                      <Section Name=""Static"">
                        <Member Name=""Stat_Bit"" Datatype=""Bool"" />
                      </Section>
                    </Sections>
                  </Member>
                  <Member Name=""message"" Datatype=""String[7]"">
                    <AttributeList>
                    </AttributeList>
                  </Member>
                  <Member Name=""SendData"" Datatype=""Array[0..4000] of Byte"">
                    <AttributeList>
                    </AttributeList>
                  </Member>
                  <Member Name=""TSEND_C_Instance"" Datatype=""TSEND_C"" Version=""3.2"">
                    <Sections>
                      <Section Name=""Input"">
                        <Member Name=""REQ"" Datatype=""Bool"" />
                        <Member Name=""CONT"" Datatype=""Bool"" />
                        <Member Name=""LEN"" Datatype=""UDInt"" />
                      </Section>
                      <Section Name=""Output"">
                        <Member Name=""DONE"" Datatype=""Bool"" />
                        <Member Name=""BUSY"" Datatype=""Bool"" />
                        <Member Name=""ERROR"" Datatype=""Bool"" />
                        <Member Name=""STATUS"" Datatype=""Word"" />
                      </Section>
                      <Section Name=""InOut"">
                        <Member Name=""CONNECT"" Datatype=""Variant"" />
                        <Member Name=""DATA"" Datatype=""Variant"" />
                        <Member Name=""ADDR"" Datatype=""Variant"" />
                        <Member Name=""COM_RST"" Datatype=""Bool"" />
                      </Section>
                      <Section Name=""Static"">
                        <Member Name=""s_state"" Datatype=""Int"" />
                        <Member Name=""s_locked"" Datatype=""Bool"" />
                        <Member Name=""s_udp"" Datatype=""Bool"" />
                        <Member Name=""s_configured"" Datatype=""Bool"" />
                        <Member Name=""s_tcon_80A3"" Datatype=""Bool"" />
                        <Member Name=""s_REQ"" Datatype=""Bool"" />
                        <Member Name=""s_ConID"" Datatype=""CONN_OUC"" />
                        <Member Name=""s_TCON"" Datatype=""TCON"" Version=""4.0"">
                          <Sections>
                            <Section Name=""Input"">
                              <Member Name=""REQ"" Datatype=""Bool"" />
                              <Member Name=""ID"" Datatype=""CONN_OUC"" />
                            </Section>
                            <Section Name=""Output"">
                              <Member Name=""DONE"" Datatype=""Bool"" />
                              <Member Name=""BUSY"" Datatype=""Bool"" />
                              <Member Name=""ERROR"" Datatype=""Bool"" />
                              <Member Name=""STATUS"" Datatype=""Word"" />
                            </Section>
                            <Section Name=""InOut"">
                              <Member Name=""CONNECT"" Datatype=""Variant"" />
                            </Section>
                            <Section Name=""Static"" />
                          </Sections>
                        </Member>
                        <Member Name=""s_TDIAG"" Datatype=""T_DIAG"" Version=""1.2"">
                          <Sections>
                            <Section Name=""Input"">
                              <Member Name=""REQ"" Datatype=""Bool"" />
                              <Member Name=""ID"" Datatype=""CONN_OUC"" />
                            </Section>
                            <Section Name=""Output"">
                              <Member Name=""DONE"" Datatype=""Bool"" />
                              <Member Name=""BUSY"" Datatype=""Bool"" />
                              <Member Name=""ERROR"" Datatype=""Bool"" />
                              <Member Name=""STATUS"" Datatype=""Word"" />
                            </Section>
                            <Section Name=""InOut"">
                              <Member Name=""RESULT"" Datatype=""Variant"" />
                            </Section>
                            <Section Name=""Static"" />
                          </Sections>
                        </Member>
                        <Member Name=""s_TDIAG_Status"" Datatype=""TDiag_Status"" Version=""1.0"">
                          <Sections>
                            <Section Name=""None"">
                              <Member Name=""InterfaceId"" Datatype=""HW_ANY"" />
                              <Member Name=""ID"" Datatype=""CONN_OUC"" />
                              <Member Name=""ConnectionType"" Datatype=""Byte"" />
                              <Member Name=""ActiveEstablished"" Datatype=""Bool"" />
                              <Member Name=""State"" Datatype=""Byte"" />
                              <Member Name=""Kind"" Datatype=""Byte"" />
                              <Member Name=""SentBytes"" Datatype=""UDInt"" />
                              <Member Name=""ReceivedBytes"" Datatype=""UDInt"" />
                            </Section>
                          </Sections>
                        </Member>
                        <Member Name=""s_TDISCON"" Datatype=""TDISCON"" Version=""2.1"">
                          <Sections>
                            <Section Name=""Input"">
                              <Member Name=""REQ"" Datatype=""Bool"" />
                              <Member Name=""ID"" Datatype=""CONN_OUC"" />
                            </Section>
                            <Section Name=""Output"">
                              <Member Name=""DONE"" Datatype=""Bool"" />
                              <Member Name=""BUSY"" Datatype=""Bool"" />
                              <Member Name=""ERROR"" Datatype=""Bool"" />
                              <Member Name=""STATUS"" Datatype=""Word"" />
                            </Section>
                            <Section Name=""InOut"" />
                            <Section Name=""Static"" />
                          </Sections>
                        </Member>
                        <Member Name=""s_TSEND"" Datatype=""TSEND"" Version=""4.0"">
                          <Sections>
                            <Section Name=""Input"">
                              <Member Name=""REQ"" Datatype=""Bool"" />
                              <Member Name=""ID"" Datatype=""CONN_OUC"" />
                              <Member Name=""LEN"" Datatype=""UDInt"" />
                            </Section>
                            <Section Name=""Output"">
                              <Member Name=""DONE"" Datatype=""Bool"" />
                              <Member Name=""BUSY"" Datatype=""Bool"" />
                              <Member Name=""ERROR"" Datatype=""Bool"" />
                              <Member Name=""STATUS"" Datatype=""Word"" />
                            </Section>
                            <Section Name=""InOut"">
                              <Member Name=""DATA"" Datatype=""Variant"" />
                              <Member Name=""ADDR"" Datatype=""Variant"" />
                            </Section>
                            <Section Name=""Static"" />
                          </Sections>
                        </Member>
                        <Member Name=""s_TUSEND"" Datatype=""TUSEND"" Version=""4.0"">
                          <Sections>
                            <Section Name=""Input"">
                              <Member Name=""REQ"" Datatype=""Bool"" />
                              <Member Name=""ID"" Datatype=""CONN_OUC"" />
                              <Member Name=""LEN"" Datatype=""UDInt"" />
                            </Section>
                            <Section Name=""Output"">
                              <Member Name=""DONE"" Datatype=""Bool"" />
                              <Member Name=""BUSY"" Datatype=""Bool"" />
                              <Member Name=""ERROR"" Datatype=""Bool"" />
                              <Member Name=""STATUS"" Datatype=""Word"" />
                            </Section>
                            <Section Name=""InOut"">
                              <Member Name=""DATA"" Datatype=""Variant"" />
                              <Member Name=""ADDR"" Datatype=""Variant"" />
                            </Section>
                            <Section Name=""Static"" />
                          </Sections>
                        </Member>
                        <Member Name=""s_TRESET"" Datatype=""T_RESET"" Version=""1.2"">
                          <Sections>
                            <Section Name=""Input"">
                              <Member Name=""REQ"" Datatype=""Bool"" />
                              <Member Name=""ID"" Datatype=""CONN_OUC"" />
                            </Section>
                            <Section Name=""Output"">
                              <Member Name=""DONE"" Datatype=""Bool"" />
                              <Member Name=""BUSY"" Datatype=""Bool"" />
                              <Member Name=""ERROR"" Datatype=""Bool"" />
                              <Member Name=""STATUS"" Datatype=""Word"" />
                            </Section>
                            <Section Name=""InOut"" />
                            <Section Name=""Static"" />
                          </Sections>
                        </Member>
                      </Section>
                    </Sections>
                  </Member>
                  <Member Name=""TSEND_C_Error"" Datatype=""Word"">
                    <AttributeList>
                    </AttributeList>
                  </Member>
                </Section>
              </Sections>
            </Member>
            <Member Name=""CpuRestartErrorTimer"" Datatype=""TOF_TIME"" Version=""1.0"" Remanence=""SetInIDB"" Accessibility=""Public"">
              <AttributeList>
                <BooleanAttribute Name=""ExternalAccessible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalVisible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalWritable"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""SetPoint"" SystemDefined=""true"">false</BooleanAttribute>
              </AttributeList>
              <Sections>
                <Section Name=""None"">
                  <Member Name=""PT"" Datatype=""Time"">
                    <AttributeList>
                    </AttributeList>
                  </Member>
                  <Member Name=""ET"" Datatype=""Time"">
                    <AttributeList>
                    </AttributeList>
                  </Member>
                  <Member Name=""IN"" Datatype=""Bool"">
                    <AttributeList>
                    </AttributeList>
                  </Member>
                  <Member Name=""Q"" Datatype=""Bool"">
                    <AttributeList>
                    </AttributeList>
                  </Member>
                </Section>
              </Sections>
            </Member>
            <Member Name=""CpuRestartErrorResetDelay"" Datatype=""TOF_TIME"" Version=""1.0"" Remanence=""SetInIDB"" Accessibility=""Public"">
              <AttributeList>
                <BooleanAttribute Name=""ExternalAccessible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalVisible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalWritable"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""SetPoint"" SystemDefined=""true"">false</BooleanAttribute>
              </AttributeList>
              <Sections>
                <Section Name=""None"">
                  <Member Name=""PT"" Datatype=""Time"">
                    <AttributeList>
                    </AttributeList>
                  </Member>
                  <Member Name=""ET"" Datatype=""Time"">
                    <AttributeList>
                    </AttributeList>
                  </Member>
                  <Member Name=""IN"" Datatype=""Bool"">
                    <AttributeList>
                    </AttributeList>
                  </Member>
                  <Member Name=""Q"" Datatype=""Bool"">
                    <AttributeList>
                    </AttributeList>
                  </Member>
                </Section>
              </Sections>
            </Member>
            <Member Name=""CpuRestartError"" Datatype=""Bool"" Remanence=""SetInIDB"" Accessibility=""Public"">
              <AttributeList>
                <BooleanAttribute Name=""ExternalAccessible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalVisible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalWritable"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""SetPoint"" SystemDefined=""true"">false</BooleanAttribute>
              </AttributeList>
            </Member>
            <Member Name=""SensorMonitoring"" Datatype=""&quot;SensorMonitoring&quot;"" Accessibility=""Public"">
              <AttributeList>
                <BooleanAttribute Name=""ExternalAccessible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalVisible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalWritable"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""SetPoint"" SystemDefined=""true"">false</BooleanAttribute>
              </AttributeList>
              <Sections>
                <Section Name=""Input"" />
                <Section Name=""Output"" />
                <Section Name=""InOut"" />
                <Section Name=""Static"">
                  <Member Name=""i"" Datatype=""Int"" />
                  <Member Name=""j"" Datatype=""Int"" />
                  <Member Name=""Centered0InterruptedRTrig"" Datatype=""R_TRIG"" Version=""1.0"">
                    <Sections>
                      <Section Name=""Input"">
                        <Member Name=""CLK"" Datatype=""Bool"" />
                      </Section>
                      <Section Name=""Output"">
                        <Member Name=""Q"" Datatype=""Bool"" />
                      </Section>
                      <Section Name=""InOut"" />
                      <Section Name=""Static"">
                        <Member Name=""Stat_Bit"" Datatype=""Bool"" />
                      </Section>
                    </Sections>
                  </Member>
                  <Member Name=""Centered1InterruptedRTrig"" Datatype=""R_TRIG"" Version=""1.0"">
                    <Sections>
                      <Section Name=""Input"">
                        <Member Name=""CLK"" Datatype=""Bool"" />
                      </Section>
                      <Section Name=""Output"">
                        <Member Name=""Q"" Datatype=""Bool"" />
                      </Section>
                      <Section Name=""InOut"" />
                      <Section Name=""Static"">
                        <Member Name=""Stat_Bit"" Datatype=""Bool"" />
                      </Section>
                    </Sections>
                  </Member>
                  <Member Name=""Centered2InterruptedRTrig"" Datatype=""R_TRIG"" Version=""1.0"">
                    <Sections>
                      <Section Name=""Input"">
                        <Member Name=""CLK"" Datatype=""Bool"" />
                      </Section>
                      <Section Name=""Output"">
                        <Member Name=""Q"" Datatype=""Bool"" />
                      </Section>
                      <Section Name=""InOut"" />
                      <Section Name=""Static"">
                        <Member Name=""Stat_Bit"" Datatype=""Bool"" />
                      </Section>
                    </Sections>
                  </Member>
                  <Member Name=""CenteredInterruptedCounter"" Datatype=""Array[1..&quot;MAX_NUM_OF_LHD&quot;, 1..&quot;MAX_NUM_OF_RP_PER_LHD&quot;, 0..2] of UInt"" />
                  <Member Name=""OrderStateBackup"" Datatype=""Array[1..&quot;MAX_NUM_OF_LHD&quot;, 1..&quot;MAX_NUM_OF_RP_PER_LHD&quot;] of UInt"" />
                </Section>
              </Sections>
            </Member>
            <Member Name=""ChargingStationError"" Datatype=""Array[1..&quot;MAX_NUM_OF_CHARGING_STATIONS&quot;] of &quot;ChargingStationError&quot;"" Accessibility=""Public"">
              <AttributeList>
                <BooleanAttribute Name=""ExternalAccessible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalVisible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalWritable"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""SetPoint"" SystemDefined=""true"">false</BooleanAttribute>
              </AttributeList>
              <Comment>
                <MultiLanguageText Lang=""de-DE"">{mmove_delete}</MultiLanguageText>
              </Comment>
              <Sections>
                <Section Name=""Input"">
                  <Member Name=""Enable"" Datatype=""Bool"" />
                  <Member Name=""ChargingModuleNoError"" Datatype=""Bool"" />
                  <Member Name=""ChargingModuleBasePositionAndNotActive"" Datatype=""Bool"" />
                </Section>
                <Section Name=""Output"" />
                <Section Name=""InOut"">
                  <Member Name=""ErrorChargingStation"" Datatype=""Bool"" />
                  <Member Name=""ErrorBasePosition"" Datatype=""Bool"" />
                </Section>
                <Section Name=""Static"">
                  <Member Name=""MMoveChargerBasePositionTimeout"" Datatype=""TON_TIME"" Version=""1.0"">
                    <Sections>
                      <Section Name=""None"">
                        <Member Name=""PT"" Datatype=""Time"" />
                        <Member Name=""ET"" Datatype=""Time"" />
                        <Member Name=""IN"" Datatype=""Bool"" />
                        <Member Name=""Q"" Datatype=""Bool"" />
                      </Section>
                    </Sections>
                  </Member>
                </Section>
              </Sections>
            </Member>
            <Member Name=""MMoveErrors"" Datatype=""Array[1..&quot;MAX_NUM_OF_MMOVE&quot;] of &quot;MMoveErrors&quot;"" Accessibility=""Public"">
              <AttributeList>
                <BooleanAttribute Name=""ExternalAccessible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalVisible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalWritable"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""SetPoint"" SystemDefined=""true"">false</BooleanAttribute>
              </AttributeList>
              <Comment>
                <MultiLanguageText Lang=""de-DE"">{mmove_delete}</MultiLanguageText>
              </Comment>
              <Sections>
                <Section Name=""Input"">
                  <Member Name=""Enable"" Datatype=""Bool"" />
                  <Member Name=""LhdCentered"" Datatype=""Bool"" />
                  <Member Name=""MMoveConnected"" Datatype=""Bool"" />
                  <Member Name=""IsMMoveSwitchedOff"" Datatype=""Bool"" />
                  <Member Name=""MMoveComMonitoringTimer"" Datatype=""DInt"" />
                  <Member Name=""ChargingModuleBasePositionAndNotActive"" Datatype=""Bool"" />
                </Section>
                <Section Name=""Output"">
                  <Member Name=""DeepsleepNotDisabled"" Datatype=""Bool"" />
                </Section>
                <Section Name=""InOut"">
                  <Member Name=""MMove"" Datatype=""&quot;TMMove&quot;"">
                    <Sections>
                      <Section Name=""None"" />
                    </Sections>
                  </Member>
                  <Member Name=""MMoveActions"" Datatype=""&quot;TMMoveActions&quot;"">
                    <Sections>
                      <Section Name=""None"" />
                    </Sections>
                  </Member>
                  <Member Name=""ErrorSafetyTimer"" Datatype=""Bool"" />
                  <Member Name=""ErrorEStop"" Datatype=""Bool"" />
                  <Member Name=""ErrorCommunicationTimeout"" Datatype=""Bool"" />
                  <Member Name=""ErrorActionTimeout"" Datatype=""Bool"" />
                  <Member Name=""ErrorNotReferenced"" Datatype=""Bool"" />
                  <Member Name=""ErrorChargeTimeout"" Datatype=""Bool"" />
                  <Member Name=""ErrorZigBee"" Datatype=""Bool"" />
                  <Member Name=""ErrorMMoveNotOnCarrier"" Datatype=""Bool"" />
                  <Member Name=""ErrorPositionUnknown"" Datatype=""Bool"" />
                  <Member Name=""ErrorStop1Active"" Datatype=""Bool"" />
                  <Member Name=""ErrorChannelEndDetected"" Datatype=""Bool"" />
                </Section>
                <Section Name=""Static"">
                  <Member Name=""OpModeBackup"" Datatype=""Byte"" />
                  <Member Name=""MMoveConnectportX2ConnectedDelay"" Datatype=""TON_TIME"" Version=""1.0"">
                    <Sections>
                      <Section Name=""None"">
                        <Member Name=""PT"" Datatype=""Time"" />
                        <Member Name=""ET"" Datatype=""Time"" />
                        <Member Name=""IN"" Datatype=""Bool"" />
                        <Member Name=""Q"" Datatype=""Bool"" />
                      </Section>
                    </Sections>
                  </Member>
                  <Member Name=""MMoveCommunicationTimeout"" Datatype=""TON_TIME"" Version=""1.0"">
                    <Sections>
                      <Section Name=""None"">
                        <Member Name=""PT"" Datatype=""Time"" />
                        <Member Name=""ET"" Datatype=""Time"" />
                        <Member Name=""IN"" Datatype=""Bool"" />
                        <Member Name=""Q"" Datatype=""Bool"" />
                      </Section>
                    </Sections>
                  </Member>
                  <Member Name=""MMoveCommunicationTimeoutTimer"" Datatype=""TON_TIME"" Version=""1.0"">
                    <Sections>
                      <Section Name=""None"">
                        <Member Name=""PT"" Datatype=""Time"" />
                        <Member Name=""ET"" Datatype=""Time"" />
                        <Member Name=""IN"" Datatype=""Bool"" />
                        <Member Name=""Q"" Datatype=""Bool"" />
                      </Section>
                    </Sections>
                  </Member>
                  <Member Name=""MMoveActionTimeout"" Datatype=""TON_TIME"" Version=""1.0"">
                    <Sections>
                      <Section Name=""None"">
                        <Member Name=""PT"" Datatype=""Time"" />
                        <Member Name=""ET"" Datatype=""Time"" />
                        <Member Name=""IN"" Datatype=""Bool"" />
                        <Member Name=""Q"" Datatype=""Bool"" />
                      </Section>
                    </Sections>
                  </Member>
                  <Member Name=""MMoveChargeActionTimeout"" Datatype=""TON_TIME"" Version=""1.0"">
                    <Sections>
                      <Section Name=""None"">
                        <Member Name=""PT"" Datatype=""Time"" />
                        <Member Name=""ET"" Datatype=""Time"" />
                        <Member Name=""IN"" Datatype=""Bool"" />
                        <Member Name=""Q"" Datatype=""Bool"" />
                      </Section>
                    </Sections>
                  </Member>
                  <Member Name=""MMoveNotReferencedOnDelay"" Datatype=""TON_TIME"" Version=""1.0"">
                    <Sections>
                      <Section Name=""None"">
                        <Member Name=""PT"" Datatype=""Time"" />
                        <Member Name=""ET"" Datatype=""Time"" />
                        <Member Name=""IN"" Datatype=""Bool"" />
                        <Member Name=""Q"" Datatype=""Bool"" />
                      </Section>
                    </Sections>
                  </Member>
                  <Member Name=""Stop1Trig"" Datatype=""R_TRIG"" Version=""1.0"">
                    <Sections>
                      <Section Name=""Input"">
                        <Member Name=""CLK"" Datatype=""Bool"" />
                      </Section>
                      <Section Name=""Output"">
                        <Member Name=""Q"" Datatype=""Bool"" />
                      </Section>
                      <Section Name=""InOut"" />
                      <Section Name=""Static"">
                        <Member Name=""Stat_Bit"" Datatype=""Bool"" />
                      </Section>
                    </Sections>
                  </Member>
                  <Member Name=""ResetTrig"" Datatype=""R_TRIG"" Version=""1.0"">
                    <Sections>
                      <Section Name=""Input"">
                        <Member Name=""CLK"" Datatype=""Bool"" />
                      </Section>
                      <Section Name=""Output"">
                        <Member Name=""Q"" Datatype=""Bool"" />
                      </Section>
                      <Section Name=""InOut"" />
                      <Section Name=""Static"">
                        <Member Name=""Stat_Bit"" Datatype=""Bool"" />
                      </Section>
                    </Sections>
                  </Member>
                  <Member Name=""MMoveNoDeepSleepTrig"" Datatype=""R_TRIG"" Version=""1.0"">
                    <Sections>
                      <Section Name=""Input"">
                        <Member Name=""CLK"" Datatype=""Bool"" />
                      </Section>
                      <Section Name=""Output"">
                        <Member Name=""Q"" Datatype=""Bool"" />
                      </Section>
                      <Section Name=""InOut"" />
                      <Section Name=""Static"">
                        <Member Name=""Stat_Bit"" Datatype=""Bool"" />
                      </Section>
                    </Sections>
                  </Member>
                  <Member Name=""ResetSafetyTimerEStopState"" Datatype=""Int"" />
                  <Member Name=""GetSafetyControllerStatusTimer"" Datatype=""Int"" />
                </Section>
              </Sections>
            </Member>
            <Member Name=""HUTargetPositionBackup"" Datatype=""DInt"" Remanence=""SetInIDB"" Accessibility=""Public"">
              <AttributeList>
                <BooleanAttribute Name=""ExternalAccessible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalVisible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalWritable"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""SetPoint"" SystemDefined=""true"">false</BooleanAttribute>
              </AttributeList>
            </Member>
            <Member Name=""OrderAckTxBusyFTrig"" Datatype=""F_TRIG"" Version=""1.0"" Accessibility=""Public"">
              <AttributeList>
                <BooleanAttribute Name=""ExternalAccessible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalVisible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalWritable"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""SetPoint"" SystemDefined=""true"">false</BooleanAttribute>
              </AttributeList>
              <Sections>
                <Section Name=""Input"">
                  <Member Name=""CLK"" Datatype=""Bool"" />
                </Section>
                <Section Name=""Output"">
                  <Member Name=""Q"" Datatype=""Bool"" />
                </Section>
                <Section Name=""InOut"" />
                <Section Name=""Static"">
                  <Member Name=""Stat_Bit"" Datatype=""Bool"" />
                </Section>
              </Sections>
            </Member>
            <Member Name=""SpeedMonitoringTD"" Datatype=""&quot;SpeedMonitoring&quot;"" Accessibility=""Public"">
              <AttributeList>
                <BooleanAttribute Name=""ExternalAccessible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalVisible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalWritable"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""SetPoint"" SystemDefined=""true"">false</BooleanAttribute>
              </AttributeList>
              <Sections>
                <Section Name=""Input"">
                  <Member Name=""Enable"" Datatype=""Bool"" />
                  <Member Name=""InvertAxis"" Datatype=""Bool"" />
                  <Member Name=""SensorPositionPlus"" Datatype=""DInt"" />
                  <Member Name=""SensorPositionMinus"" Datatype=""DInt"" />
                  <Member Name=""MaxSpeed"" Datatype=""Int"" />
                  <Member Name=""ActSpeed"" Datatype=""Int"" />
                  <Member Name=""ActPosition"" Datatype=""DInt"" />
                  <Member Name=""PosWindow"" Datatype=""DInt"" />
                  <Member Name=""Factor70Percent"" Datatype=""Int"" />
                </Section>
                <Section Name=""Output"" />
                <Section Name=""InOut"">
                  <Member Name=""Configuration"" Datatype=""&quot;SafetyAxis&quot;"">
                    <Sections>
                      <Section Name=""None"" />
                    </Sections>
                  </Member>
                  <Member Name=""Inputs"" Datatype=""&quot;SafetyAxis&quot;"">
                    <Sections>
                      <Section Name=""None"" />
                    </Sections>
                  </Member>
                  <Member Name=""Error"" Datatype=""Bool"" />
                  <Member Name=""Status"" Datatype=""Byte"" />
                </Section>
                <Section Name=""Static"" />
              </Sections>
            </Member>
            <Member Name=""SpeedMonitoringHU1"" Datatype=""&quot;SpeedMonitoring&quot;"" Accessibility=""Public"">
              <AttributeList>
                <BooleanAttribute Name=""ExternalAccessible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalVisible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalWritable"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""SetPoint"" SystemDefined=""true"">false</BooleanAttribute>
              </AttributeList>
              <Sections>
                <Section Name=""Input"">
                  <Member Name=""Enable"" Datatype=""Bool"" />
                  <Member Name=""InvertAxis"" Datatype=""Bool"" />
                  <Member Name=""SensorPositionPlus"" Datatype=""DInt"" />
                  <Member Name=""SensorPositionMinus"" Datatype=""DInt"" />
                  <Member Name=""MaxSpeed"" Datatype=""Int"" />
                  <Member Name=""ActSpeed"" Datatype=""Int"" />
                  <Member Name=""ActPosition"" Datatype=""DInt"" />
                  <Member Name=""PosWindow"" Datatype=""DInt"" />
                  <Member Name=""Factor70Percent"" Datatype=""Int"" />
                </Section>
                <Section Name=""Output"" />
                <Section Name=""InOut"">
                  <Member Name=""Configuration"" Datatype=""&quot;SafetyAxis&quot;"">
                    <Sections>
                      <Section Name=""None"" />
                    </Sections>
                  </Member>
                  <Member Name=""Inputs"" Datatype=""&quot;SafetyAxis&quot;"">
                    <Sections>
                      <Section Name=""None"" />
                    </Sections>
                  </Member>
                  <Member Name=""Error"" Datatype=""Bool"" />
                  <Member Name=""Status"" Datatype=""Byte"" />
                </Section>
                <Section Name=""Static"" />
              </Sections>
            </Member>
            <Member Name=""SpeedMonitoringHU2"" Datatype=""&quot;SpeedMonitoring&quot;"" Accessibility=""Public"">
              <AttributeList>
                <BooleanAttribute Name=""ExternalAccessible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalVisible"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""ExternalWritable"" SystemDefined=""true"">false</BooleanAttribute>
                <BooleanAttribute Name=""SetPoint"" SystemDefined=""true"">false</BooleanAttribute>
              </AttributeList>
              <Sections>
                <Section Name=""Input"">
                  <Member Name=""Enable"" Datatype=""Bool"" />
                  <Member Name=""InvertAxis"" Datatype=""Bool"" />
                  <Member Name=""SensorPositionPlus"" Datatype=""DInt"" />
                  <Member Name=""SensorPositionMinus"" Datatype=""DInt"" />
                  <Member Name=""MaxSpeed"" Datatype=""Int"" />
                  <Member Name=""ActSpeed"" Datatype=""Int"" />
                  <Member Name=""ActPosition"" Datatype=""DInt"" />
                  <Member Name=""PosWindow"" Datatype=""DInt"" />
                  <Member Name=""Factor70Percent"" Datatype=""Int"" />
                </Section>
                <Section Name=""Output"" />
                <Section Name=""InOut"">
                  <Member Name=""Configuration"" Datatype=""&quot;SafetyAxis&quot;"">
                    <Sections>
                      <Section Name=""None"" />
                    </Sections>
                  </Member>
                  <Member Name=""Inputs"" Datatype=""&quot;SafetyAxis&quot;"">
                    <Sections>
                      <Section Name=""None"" />
                    </Sections>
                  </Member>
                  <Member Name=""Error"" Datatype=""Bool"" />
                  <Member Name=""Status"" Datatype=""Byte"" />
                </Section>
                <Section Name=""Static"" />
              </Sections>
            </Member>
          </Section>
          <Section Name=""Temp"">
            <Member Name=""helpbool"" Datatype=""Bool"">
              <AttributeList>
              </AttributeList>
            </Member>
            <Member Name=""helpBool1"" Datatype=""Bool"">
              <AttributeList>
              </AttributeList>
            </Member>
            <Member Name=""helpbool2"" Datatype=""Bool"">
              <AttributeList>
              </AttributeList>
            </Member>
            <Member Name=""FehlerGrundstellungTGs"" Datatype=""Bool"">
              <AttributeList>
              </AttributeList>
            </Member>
            <Member Name=""helpDint1"" Datatype=""DInt"">
              <AttributeList>
              </AttributeList>
            </Member>
            <Member Name=""helpDint2"" Datatype=""DInt"">
              <AttributeList>
              </AttributeList>
            </Member>
            <Member Name=""helpDint3"" Datatype=""DInt"">
              <AttributeList>
              </AttributeList>
            </Member>
            <Member Name=""helpDint4"" Datatype=""DInt"">
              <AttributeList>
              </AttributeList>
            </Member>
            <Member Name=""HolenLinks"" Datatype=""Bool"">
              <AttributeList>
              </AttributeList>
            </Member>
            <Member Name=""HolenRechts"" Datatype=""Bool"">
              <AttributeList>
              </AttributeList>
            </Member>
            <Member Name=""BringenLinks"" Datatype=""Bool"">
              <AttributeList>
              </AttributeList>
            </Member>
            <Member Name=""BringenRechts"" Datatype=""Bool"">
              <AttributeList>
              </AttributeList>
            </Member>
            <Member Name=""SpaltUeberbruecken"" Datatype=""Bool"">
              <AttributeList>
              </AttributeList>
            </Member>
            <Member Name=""MitteUeberbruecken"" Datatype=""Bool"">
              <AttributeList>
              </AttributeList>
            </Member>
            <Member Name=""LastwechselHolen"" Datatype=""Bool"">
              <AttributeList>
              </AttributeList>
            </Member>
            <Member Name=""LastwechselBringen"" Datatype=""Bool"">
              <AttributeList>
              </AttributeList>
            </Member>
            <Member Name=""autoOff1Only"" Datatype=""Bool"">
              <AttributeList>
              </AttributeList>
            </Member>
            <Member Name=""autoOff2Only"" Datatype=""Bool"">
              <AttributeList>
              </AttributeList>
            </Member>
            <Member Name=""autoOff3Only"" Datatype=""Bool"">
              <AttributeList>
              </AttributeList>
            </Member>
            <Member Name=""autoOff4Only"" Datatype=""Bool"">
              <AttributeList>
              </AttributeList>
            </Member>
            <Member Name=""enableMMoveChargingStationErrors"" Datatype=""Array[1..&quot;MAX_NUM_OF_CHARGING_STATIONS&quot;] of Bool"">
              <AttributeList>
              </AttributeList>
            </Member>
            <Member Name=""automaticInstanceEnabled"" Datatype=""Bool"">
              <AttributeList>
              </AttributeList>
            </Member>
            <Member Name=""huTargetPositionChanged"" Datatype=""Bool"">
              <AttributeList>
              </AttributeList>
            </Member>
            <Member Name=""enableMMoveErrors"" Datatype=""Bool"">
              <AttributeList>
              </AttributeList>
            </Member>
            <Member Name=""autoResetStatusCodePickInfeed"" Datatype=""Bool"">
              <AttributeList>
              </AttributeList>
            </Member>
            <Member Name=""autoResetStatusCodeDropOutfeed"" Datatype=""Bool"">
              <AttributeList>
              </AttributeList>
            </Member>
            <Member Name=""autoResetStatusCode"" Datatype=""Bool"">
              <AttributeList>
              </AttributeList>
            </Member>
            <Member Name=""valueRangeError"" Datatype=""Bool"">
              <AttributeList>
              </AttributeList>
            </Member>
            <Member Name=""speedMonitoringInputs"" Datatype=""&quot;SafetyAxis&quot;"">
              <AttributeList>
              </AttributeList>
              <Sections>
                <Section Name=""None"">
                  <Member Name=""Standstill"" Datatype=""Bool"">
                    <AttributeList>
                    </AttributeList>
                  </Member>
                  <Member Name=""V3MeterPerMin"" Datatype=""Bool"">
                    <AttributeList>
                    </AttributeList>
                  </Member>
                  <Member Name=""V30MeterPerMin"" Datatype=""Bool"">
                    <AttributeList>
                    </AttributeList>
                  </Member>
                  <Member Name=""V70Percent"" Datatype=""Bool"">
                    <AttributeList>
                    </AttributeList>
                  </Member>
                  <Member Name=""V110Percent"" Datatype=""Bool"">
                    <AttributeList>
                    </AttributeList>
                  </Member>
                  <Member Name=""LimitSwitch0"" Datatype=""Bool"">
                    <AttributeList>
                    </AttributeList>
                  </Member>
                  <Member Name=""LimitSwitch1"" Datatype=""Bool"">
                    <AttributeList>
                    </AttributeList>
                  </Member>
                  <Member Name=""LimitSwitch2"" Datatype=""Bool"">
                    <AttributeList>
                    </AttributeList>
                  </Member>
                  <Member Name=""SpeedMonitoringSensorPlus"" Datatype=""Bool"">
                    <AttributeList>
                    </AttributeList>
                  </Member>
                  <Member Name=""SpeedMonitoringSensorMinus"" Datatype=""Bool"">
                    <AttributeList>
                    </AttributeList>
                  </Member>
                  <Member Name=""GovernorRope"" Datatype=""Bool"">
                    <AttributeList>
                    </AttributeList>
                  </Member>
                  <Member Name=""OverspeedGovernor"" Datatype=""Bool"">
                    <AttributeList>
                    </AttributeList>
                  </Member>
                  <Member Name=""SafeTorqueOff"" Datatype=""Bool"">
                    <AttributeList>
                    </AttributeList>
                  </Member>
                  <Member Name=""BrakeVoltageOff"" Datatype=""Bool"">
                    <AttributeList>
                    </AttributeList>
                  </Member>
                  <Member Name=""SpeedMonitoringOk"" Datatype=""Bool"">
                    <AttributeList>
                    </AttributeList>
                  </Member>
                  <Member Name=""EmergencyLimitSwitchOk"" Datatype=""Bool"">
                    <AttributeList>
                    </AttributeList>
                  </Member>
                  <Member Name=""MotionRangeLimitSwitchOk"" Datatype=""Bool"">
                    <AttributeList>
                    </AttributeList>
                  </Member>
                  <Member Name=""AxisOnWithOffDelay"" Datatype=""Bool"">
                    <AttributeList>
                    </AttributeList>
                  </Member>
                  <Member Name=""PowerOffWarning"" Datatype=""Bool"">
                    <AttributeList>
                    </AttributeList>
                  </Member>
                  <Member Name=""OnRequest"" Datatype=""Bool"">
                    <AttributeList>
                    </AttributeList>
                  </Member>
                </Section>
              </Sections>
            </Member>
            <Member Name=""onlyOneAutoKeyIsOff"" Datatype=""Bool"">
              <AttributeList>
              </AttributeList>
            </Member>
            <Member Name=""enable"" Datatype=""Bool"">
              <AttributeList>
              </AttributeList>
            </Member>
          </Section>
          <Section Name=""Constant"" />
        </Sections>
      </Interface>
      <IsConsistent ReadOnly=""true"">true</IsConsistent>
      <IsKnowHowProtected ReadOnly=""true"">false</IsKnowHowProtected>
      <ISMultiInstanceCapable ReadOnly=""true"">true</ISMultiInstanceCapable>
      <IsWriteProtected ReadOnly=""true"">false</IsWriteProtected>
      <MemoryLayout>Standard</MemoryLayout>
      <Name>ErrorHandler</Name>
      <Number>5800</Number>
      <ParameterPassing>false</ParameterPassing>
      <PLCSimAdvancedSupport ReadOnly=""true"">false</PLCSimAdvancedSupport>
      <ProgrammingLanguage>STL</ProgrammingLanguage>
      <UDABlockProperties />
      <UDAEnableTagReadback>false</UDAEnableTagReadback>
    </AttributeList>
    <ObjectList>
      <MultilingualText ID=""1"" CompositionName=""Comment"">
        <ObjectList>
          <MultilingualTextItem ID=""2"" CompositionName=""Items"">
            <AttributeList>
              <Culture>en-GB</Culture>
              <Text />
            </AttributeList>
          </MultilingualTextItem>
          <MultilingualTextItem ID=""3"" CompositionName=""Items"">
            <AttributeList>
              <Culture>de-DE</Culture>
              <Text>This function block generates until here unhandled errors (incl. order errors). 
Furthermore this is the only position in the program, where error classes for 
the machine and each axis are build.</Text>
            </AttributeList>
          </MultilingualTextItem>
        </ObjectList>
      </MultilingualText>
      <SW.Blocks.CompileUnit ID=""4"" CompositionName=""CompileUnits"">
      </SW.Blocks.CompileUnit>
      <MultilingualText ID=""51"" CompositionName=""Title"">
        <ObjectList>
          <MultilingualTextItem ID=""52"" CompositionName=""Items"">
            <AttributeList>
              <Culture>en-GB</Culture>
              <Text />
            </AttributeList>
          </MultilingualTextItem>
          <MultilingualTextItem ID=""53"" CompositionName=""Items"">
            <AttributeList>
              <Culture>de-DE</Culture>
              <Text>ErrorHandler</Text>
            </AttributeList>
          </MultilingualTextItem>
        </ObjectList>
      </MultilingualText>
    </ObjectList>
  </SW.Blocks.FB>
</Document>
";
            XmlDocument xmlDoc = new XmlDocument();
            XmlNamespaceManager ns = new XmlNamespaceManager(xmlDoc.NameTable);
            ns.AddNamespace("smns", "http://www.siemens.com/automation/Openness/SW/NetworkSource/FlgNet/v3");
            ns.AddNamespace("smns2", "http://www.siemens.com/automation/Openness/SW/Interface/v3");
            xmlDoc.LoadXml(test);

            var nodes = xmlDoc.SelectNodes("//*[local-name()='Member'][contains(@Datatype,'\"')]//*[local-name()='Sections']");
        }
	}
}
