---
title: "Example Report on Report Tooling"
author: [Your Name]
titlepage: true
mainfont: "Georgia"
sansfont: "Georgia"
abstract: |
  This is an abstract.
...

# Crinis mixtaque factisque ille

## Aut nunc furori ad latarumque Philomela

Lorem markdownum includite volenti monticolae videre vocem hac sparsit puta [@texbook]
gelidis vestros egressus sex. Undis eris per auguris armis. Est saevior pater.
Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet.

See also [Image with Caption](#image-with-caption).

## Gaudet Silenus iuvenis

Mulciber denique faces ingratus, in umeros umeri cum, iram ira custos non.
Pariterque admissa nubes, in ait ecce setae summis sacrorum me gaudete tellus.
Ille tu perire ille, artificis caede.

```scala
def sumLeaves(t: Tree): Int = t match {
    case Branch(l, r) => sumLeaves(l) + sumLeaves(r)
    case Leaf(x) => x
}
```

Cephea rector minorque, quem corpora,
Argus. Superi hoc tenuavit timebant ossibus totque non serpere animo corpore
superas gelidae, comitate deus Iunonigenaeque
pectora.

- Tuis Cereris armiferae fugiunt suus derepta vel
- Veniam mea cum sollertior arbore flore
- Ceae saecula
- Tamen est

# Vagata eiectatamque sidera satis reducet

## Talem ex aliquo ingemuit

Lorem markdownum solus miserabile sitae. Tantum Syron limenque cupidine: litore
modo coniuge: in huc, illo crimen novena, vocisque gratia, quae. Sua manusque
patris nec meritorum pedibus hominis virgine, ruere tamen virtus aliter. Tunc
ego. Solitaque remittant fagus omnia eat.

$$r_d^i(t+1) = \min\{r_s,\max\{0, r_d^i(t) + \beta(n_t - \lvert N_i(t)\rvert)\}\}$$

Obstitit silentia et novi non, huic metitur, coronantur lucos. Bracchia aura;
donis quod volucris illi futurae, ut
*venturorumque tellus* arma: saxumque.

# Tables and Images

## Image with Caption

![Nam liber tempor cum soluta nobis eleifend option congue nihil imperdiet doming id quod mazim placerat facer possim assum. Lorem ipsum dolor sit amet, consectetuer adipiscing elit, sed diam nonummy nibh euismod tincidunt ut laoreet dolore magna aliquam erat volutpat.](image.png)

## Markdown Table with Caption

Lorem markdownum Letoia, et alios: figurae flectentem annis aliquid Peneosque abesse, obstat gravitate. Obscura atque coniuge, per de coniunx, sibi medias
commentaque virgine anima tamen comitemque petis, sed. In Amphion vestros
hamos ire arceor mandere spicula, in licet aliquando.

| Test Nr. | Position | Radius | Rot | Grün | Blau | beste Fitness | Abweichung |
|---|---|---|---|---|---|---|---|
| 1 |  20 % |  20 % |  20 % |  20 % |  20 % |  7,5219 |  0,9115 |
| 2 |   0 % |  25 % |  25 % |  25 % |  25 % |  8,0566 |  1,4462 |
| 3 |   0 % |   0 % |  33 % |  33 % |  33 % |  8,7402 |  2,1298 |
| 4 |  50 % |  20 % |  10 % |  10 % |  10 % |  6,6104 |  0,0000 |
| 5 |  70 % |   0 % |  10 % |  10 % |  10 % |  7,0696 |  0,4592 |
| 6 |  20 % |  50 % |  10 % |  10 % |  10 % |  7,0034 |  0,3930 |
| 7 |  40 % |  15 % |  15 % |  15 % |  15 % |  6,9122 |  0,3018 |

Table:  Demonstration of simple table syntax.

Porrigitur et Pallas nuper longusque cratere habuisse sepulcro pectore fertur.
Laudat ille auditi; vertitur iura tum nepotis causa; motus. Diva virtus! Acrota destruitis vos iubet quo et classis excessere Scyrumve spiro subitusque mente Pirithoi abstulit, lapides.

## LaTeX Table with Caption

At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr.

\begin{longtable}[]{llllllll}
\caption[Nam liber tempor cum soluta nobis eleifend option congue.]{Nam liber tempor cum soluta nobis eleifend option congue nihil imperdiet doming id quod mazim placerat facer possim assum. Lorem ipsum dolor sit amet, consectetuer adipiscing elit, sed diam nonummy nibh euismod tincidunt ut laoreet dolore magna aliquam erat volutpat.} \\
\toprule
Test Nr. & Position & Radius & Rot & Grün & Blau &
beste Fitness & Abweichung\tabularnewline
\midrule
\endhead
1 & 20 \% & 20 \% & 20 \% & 20 \% & 20 \% & 7,5219 &
0,9115\tabularnewline
2 & 0 \% & 25 \% & 25 \% & 25 \% & 25 \% & 8,0566 &
1,4462\tabularnewline
3 & 0 \% & 0 \% & 33 \% & 33 \% & 33 \% & 8,7402 & 2,1298\tabularnewline
4 & 50 \% & 20 \% & 10 \% & 10 \% & 10 \% & 6,6104 &
0,0000\tabularnewline
5 & 70 \% & 0 \% & 10 \% & 10 \% & 10 \% & 7,0696 &
0,4592\tabularnewline
6 & 20 \% & 50 \% & 10 \% & 10 \% & 10 \% & 7,0034 &
0,3930\tabularnewline
\bottomrule
\end{longtable}

At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr.
