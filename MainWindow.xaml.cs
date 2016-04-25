﻿using Microsoft.Kinect;
using Microsoft.Speech.Recognition;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;

namespace Kinect2Server
{
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            setKinectSensor();
            this.publisher = new NetworkPublisher();
            this.publisher.Bind("33405");

            this.initializeSR();
            this.initializeGR();
            InitializeComponent();
        }

        private NetworkPublisher publisher;
        private KinectSensor kinectSensor;
        private KinectAudioStream convertStream;
        private SpeechRecognition sr;
        private GestureRecognition gr;

        private void initializeSR()
        {
            this.sr = new SpeechRecognition(this.kinectSensor, this.publisher, this.convertStream);
        }

        private void initializeGR()
        {
            this.gr = new GestureRecognition(this.kinectSensor, this.publisher);
        }

        private void setKinectSensor()
        {
            // Only one sensor is supported
            this.kinectSensor = KinectSensor.GetDefault();

            if (this.kinectSensor != null)
            {
                // open the sensor
                this.kinectSensor.Open();

                // grab the audio stream
                IReadOnlyList<AudioBeam> audioBeamList = this.kinectSensor.AudioSource.AudioBeams;
                System.IO.Stream audioStream = audioBeamList[0].OpenInputStream();

                // create the convert stream
                this.convertStream = new KinectAudioStream(audioStream);

            }
            else
            {
                // on failure, set the status text and close the application
                //this.statusBarText.Text = Properties.Resources.NoKinectReady;
                System.Threading.Thread.Sleep(10000);
            }
        }

        public SpeechRecognition SpeechRecogniton
        {
            get
            {
                return this.sr;
            }
        }

        public GestureRecognition GestureRecognition
        {
            get
            {
                return this.gr;
            }
        }

        public KinectSensor KinectSensor
        {
            get
            {
                return this.kinectSensor;
            }
        }

        public void addSRList(EventHandler<SpeechRecognizedEventArgs> f1, EventHandler<SpeechRecognitionRejectedEventArgs> f2)
        {
            sr.addSRListener(f1,f2);
        }

        public void addGRList(EventHandler<BodyFrameArrivedEventArgs> f1)
        {
            gr.addGRListener(f1);
        }

        public void removeGRList(EventHandler<BodyFrameArrivedEventArgs> f1)
        {
            gr.removeGRListener(f1);
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void WindowClosing(object sender, CancelEventArgs e)
        {
            if (null != this.convertStream)
            {
                this.convertStream.SpeechActive = false;
            }

            if (sr.isSpeechEngineSet())
            {
                sr.disableSpeechEngine();
            }

            if (null != this.kinectSensor)
            {
                this.kinectSensor.Close();
                this.kinectSensor = null;
            }
        }
    }
}