#!/usr/bin/perl

@files_out = ('messages.pot', 'bg.po', 'cs.po', 'de.po', 'es.po', 'fr.po', 'it.po', 'ja.po', 'kk.po', 'nl.po', 'pl.po', 'pt_BR.po', 'ru.po', 'sv.po', 'tr.po');

foreach $file_out (@files_out)
{
  $file_in = $file_out;
  if (($file_out =~ "messages.pot") && $ARGV[0])
  {
    $file_in = $ARGV[0];
  }

  open(INPUT," < $file_in") or die;
  @input_array = <INPUT>;
  close(INPUT);

  $input_scalar = join("",@input_array);
  $input_scalar =~ s/(\n\n)(#\..*\n)?(#:.*\nmsgid "A graphical frontend for Hamachi.")/$1#. Description used in about dialog.\n$3/g;
  $input_scalar =~ s/(\n\n)(#\..*\n)?(#:.*\nmsgid "Join and create local networks over the Internet")/$1#. Description used in application launchers.\n$3/g;
  $input_scalar =~ s/(\n\n)(#\..*\n)?(#:.*\nmsgid "_Evict")/$1#. This action removes the selected member from the network.\n$3/g;
  $input_scalar =~ s/(\n\n)(#\..*\n)?(#:.*\nmsgid "Desktop")/$1#. In this context meaning "work environment".\n$3/g;
  $input_scalar =~ s/(\n\n)(#\..*\n)?(#:.*\nmsgid "C_onnect \(\%S\)")/$1#. Please use the same text and shortcut as the stock connect button.\n$3/g;
  $input_scalar =~ s/(\n\n)(#\..*\n)?(#:.*\nmsgid "\{0\} came online in the network \{1\}")/$1#. Notification bubble. For example: "T-800 came online in the network Skynet".\n$3/g;
  $input_scalar =~ s/(\n\n)(#\..*\n)?(#:.*\nmsgid "\{0\} went offline in the network \{1\}")/$1#. Notification bubble. For example: "T-800 went offline in the network Skynet".\n$3/g;
  $input_scalar =~ s/(\n\n)(#\..*\n)?(#:.*\nmsgid "\{0\} joined the network \{1\}")/$1#. Notification bubble. For example: "T-800 joined the network Skynet".\n$3/g;
  $input_scalar =~ s/(\n\n)(#\..*\n)?(#:.*\nmsgid "\{0\} left the network \{1\}")/$1#. Notification bubble. For example: "T-800 left the network Skynet".\n$3/g;
  $input_scalar =~ s/(\n\n)(#\..*\n)?(#:.*\nmsgid "\{0\} came online in the network \{1\} and \{2\} other network")/$1#. Notification bubble. For example: "T-800 came online in the network Skynet and 1 other network".\n$3/g;
  $input_scalar =~ s/(\n\n)(#\..*\n)?(#:.*\nmsgid "\{0\} went offline in the network \{1\} and \{2\} other network")/$1#. Notification bubble. For example: "T-800 went offline in the network Skynet and 1 other network".\n$3/g;
  $input_scalar =~ s/(\n\n)(#\..*\n)?(#:.*\nmsgid "\{0\} joined the network \{1\} and \{2\} other network")/$1#. Notification bubble. For example: "T-800 joined the network Skynet and 1 other network".\n$3/g;
  $input_scalar =~ s/(\n\n)(#\..*\n)?(#:.*\nmsgid "\{0\} left the network \{1\} and \{2\} other network")/$1#. Notification bubble. For example: "T-800 left the network Skynet and 1 other network".\n$3/g;

  open(OUTPUT," > $file_out") or die;
  print(OUTPUT $input_scalar);
  close(OUTPUT);
}

