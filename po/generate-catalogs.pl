#!/usr/bin/perl

@locales = ('ast', 'bg', 'cs', 'de', 'es', 'fr', 'id', 'it', 'ja', 'kk', 'nl', 'pl', 'pt_BR', 'ru', 'sk', 'sv', 'tr', 'uk');

foreach $locale (@locales)
{
  system("msgfmt $locale.po -o haguichi.mo");
  system("mv haguichi.mo ../locale/$locale/LC_MESSAGES/haguichi.mo");
}

