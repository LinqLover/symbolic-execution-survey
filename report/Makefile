REPORT_FILE=report

PANDOC=pandoc

PANDOC_OPTIONS=--from markdown --template ./eisvogel.tex --number-sections --listings --bibliography=bibliography.bib --biblatex --filter pandoc-fignos

OUTPUT_FOLDER=output

PDFLATEX=pdflatex -output-directory=$(OUTPUT_FOLDER)

all : tex pdf full-pdf .output-folder
.PHONY: all

pdf : tex .output-folder
	$(PDFLATEX) $(REPORT_FILE).tex

.output-folder:
	mkdir -p $(OUTPUT_FOLDER)

tex :
	$(PANDOC) $(REPORT_FILE).md -o $(REPORT_FILE).tex $(PANDOC_OPTIONS)

full-pdf : tex .output-folder
	$(PDFLATEX) $(REPORT_FILE).tex
	biber $(OUTPUT_FOLDER)/$(REPORT_FILE)
	$(PDFLATEX) $(REPORT_FILE).tex
	$(PDFLATEX) $(REPORT_FILE).tex

clean :
	rm -rf $(REPORT_FILE).tex
	rm -rf $(OUTPUT_FOLDER)
