declare var d3: typeof import("d3");

function canvasResize() {
    const canvas = document.getElementById("quizChart") as HTMLCanvasElement;
    canvas.width = window.innerWidth * 0.8;
    canvas.height = window.innerHeight * 0.8;
    canvas.focus();

    window.addEventListener('resize', () => {
        console.log("resize");
        canvas.width = window.innerWidth * 0.8;
        canvas.height = window.innerHeight * 0.8;
    });
}

function renderQuizResult(data) {
    let svg = d3.select("svg"),
        margin = {
            top: 20,
            right: 20,
            bottom: 30,
            left: 50
        },
        width = +svg.attr("width") - margin.left - margin.right,
        height = +svg.attr("height") - margin.top - margin.bottom,
        g = svg.append("g").attr("transform", "translate(" + margin.left + "," + margin.top + ")");

    let x = d3.scaleBand()
        .rangeRound([0, width])
        .padding(0.1);

    let y = d3.scaleLinear()
        .rangeRound([height, 0]);

    x.domain(data.map(function (d) {
        return d.name;
    }));
    y.domain([0, d3.max(data, function (d:any) {
        return Number(d.count);
    })]);

    g.append("g")
        .attr("transform", "translate(0," + height + ")")
        .call(d3.axisBottom(x))

    g.append("g")
        .call(d3.axisLeft(y))
        .append("text")
        .attr("fill", "#000")
        .attr("transform", "rotate(-90)")
        .attr("y", 6)
        .attr("dy", "0.71em")
        .attr("text-anchor", "end")
        .text("Count");

    g.selectAll(".quiz-results-bar")
        .data(data)
        .enter().append("rect")
        .attr("class", "quiz-results-bar")
        .attr("x", function (d:any) {
            return x(d.name);
        })
        .attr("y", function (d:any) {
            return y(Number(d.count));
        })
        .attr("width", x.bandwidth())
        .attr("height", function (d:any) {
            return height - y(Number(d.count));
        });
}

let result = [
    { name: "Text 1", count:  16},
    { name: "Text 3", count:  12},
    { name: "Text 2", count:  5},
    { name: "Text 4", count: 2 }
];

canvasResize();
renderQuizResult(result);
